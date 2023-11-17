using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Configuration;

public class DatabaseHelper
{
    string Server { get; }
    string AdminPassword { get; }
    SqlConnection SqlConnection { get; }

    public DatabaseHelper(string server, string adminPassword)
    {
        Server = server;
        AdminPassword = adminPassword;
        SqlConnection = new SqlConnection(
            $"Server={server};Initial Catalog=master;User id=sa;Password={adminPassword};TrustServerCertificate=true;"
        );
    }

    public async Task<string> CreateDatabaseAndGetConnectionString(
        string databaseName,
        string databaseUser,
        string databasePassword
    )
    {
        Log.Information("Connecting to SQL Server...");
        await SqlConnection.OpenAsync();
        Log.Information("Successfully connected to SQL Server");

        Log.Information("Creating database {databaseName} on {Server}...", databaseName, Server);
        var createDatabaseCommand = new SqlCommand(
            $"CREATE DATABASE {databaseName}",
            SqlConnection
        );
        await createDatabaseCommand.ExecuteNonQueryAsync();
        Log.Information("Successfully created database");

        Log.Information("Creating login {databaseUser} on {Server}...", databaseName, Server);
        var createLoginCommand = new SqlCommand(
            $"CREATE LOGIN {databaseUser} WITH PASSWORD = '{databasePassword}', DEFAULT_DATABASE={databaseName}, CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;",
            SqlConnection
        );
        await createLoginCommand.ExecuteNonQueryAsync();
        Log.Information("Successfully created login");

        Log.Information(
            "Creating user {databaseUser} on {Server}/{databaseName}...",
            databaseName,
            Server,
            databaseName
        );
        var createUserCommand = new SqlCommand(
            $"USE {databaseName};"
                + $"CREATE USER {databaseUser} FOR LOGIN {databaseUser};"
                + $"ALTER ROLE db_datareader ADD MEMBER {databaseUser};"
                + $"ALTER ROLE db_datawriter ADD MEMBER {databaseUser};"
                + $"ALTER ROLE db_ddladmin ADD MEMBER {databaseUser};",
            SqlConnection
        );
        await createUserCommand.ExecuteNonQueryAsync();
        Log.Information("Successfully created user");

        return $"Server={Server};Database={databaseName};User Id={databaseUser};Password={databasePassword};Encrypt=False;";
    }

    public async Task WaitForSqlServerResponse()
    {
        const int maxConnectionAttempts = 10;
        var connectionAttempts = 0;

        Log.Information("Waiting for SQL Server to start up...");

        while (connectionAttempts < maxConnectionAttempts)
        {
            try
            {
                await SqlConnection.OpenAsync();
                Log.Information(
                    "Connection Attempt {ConnectionAttempt} - Success connecting to {Server}!",
                    connectionAttempts + 1,
                    Server
                );
                return;
            }
            catch (SqlException e)
            {
                Log.Information(
                    "Connection Attempt {ConnectionAttempts} - {Message}",
                    connectionAttempts + 1,
                    e.Message
                );
                connectionAttempts++;
                await Task.Delay(5000);
            }
        }

        throw new Exception("Failed to connect to SQL Server");
    }
}
