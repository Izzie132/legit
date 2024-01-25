using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Testing;
using Respawn;
using Web.Configuration;
using Web.Database;

namespace WebTests;

[Collection(nameof(BaseWebTest))]
public class BaseWebTest : TestClass<WebTestFixture>, IDisposable
{
    protected HttpClient Client => Fx.Client;
    protected static FakeClock FakeClock => WebTestFixture.FakeClock;

    protected DataContext DataContext;

    private readonly List<IServiceScope> serviceScopes = new();

    private static string? connectionString;
    private static SqlConnection? connection;
    private static Respawner? respawner;

    public BaseWebTest(WebTestFixture f, ITestOutputHelper o)
        : base(f, o)
    {
        DataContext = ResolveService<DataContext>();
    }

    protected T ResolveService<T>()
        where T : notnull
    {
        var scope = Fx.Services.CreateScope();
        serviceScopes.Add(scope);
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    protected void AddEntity<T>(T entity)
        where T : class => DataContext.AddEntity(entity);

    protected void AddEntities<T>(IEnumerable<T> entities)
        where T : class => DataContext.AddEntities(entities);

    protected void AddEntities<T>(params T[] entities)
        where T : class => DataContext.AddEntities(entities);

    private async Task ResetDatabase()
    {
        if (connectionString == null)
        {
            var options = ResolveService<IOptions<ProjectNameOptions>>().Value;
            connectionString = options.ConnectionString;
        }

        if (connection == null)
        {
            connection ??= new SqlConnection(connectionString);
            await connection.OpenAsync();
        }

        respawner ??= await Respawner.CreateAsync(connection);

        await respawner.ResetAsync(connection);
    }

    public void Dispose()
    {
        ResetDatabase().Wait();

        foreach (var scope in serviceScopes)
        {
            scope.Dispose();
        }

        FakeClock.Reset(SystemClock.Instance.GetCurrentInstant());
    }
}
