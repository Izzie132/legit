using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Microsoft.Data.SqlClient;

namespace Migrations;

public class AzureSqlConnectionManager(string connectionString, string azureAccessToken)
    : DatabaseConnectionManager(
        new DelegateConnectionFactory(
            (Func<IUpgradeLog, DatabaseConnectionManager, IDbConnection>)(
                (log, dbManager) =>
                {
                    var sqlConnection = new SqlConnection(connectionString);

                    if (dbManager.IsScriptOutputLogged)
                    {
                        sqlConnection.InfoMessage += (SqlInfoMessageEventHandler)(
                            (_, e) => log.WriteInformation("{0}", e.Message)
                        );
                    }

                    sqlConnection.AccessToken = azureAccessToken;

                    return sqlConnection;
                }
            )
        )
    )
{
    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents) =>
        new SqlCommandSplitter().SplitScriptIntoCommands(scriptContents);
}
