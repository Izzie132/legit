using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Testing;
using Respawn;
using Web.Configuration;
using Web.Database;

namespace WebTests;

[Collection(nameof(BaseWebTest))]
public class BaseWebTest : TestBase<WebTestFixture>, IAsyncDisposable
{
    private readonly WebTestFixture fixture;
    protected HttpClient Client => fixture.Client;
    protected FakeClock FakeClock => (FakeClock)ResolveService<IClock>();

    protected DataContext DataContext { get; }

    private static string? connectionString;
    private static SqlConnection? connection;
    private static Respawner? respawner;

    protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    private readonly List<IServiceScope> serviceScopes = [];

    protected BaseWebTest(WebTestFixture fixture)
    {
        this.fixture = fixture;

        DataContext = ResolveService<DataContext>();

        // Disable query tracking for tests, so that we don't end up with stale data when checking the database for updates
        DataContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public async ValueTask DisposeAsync()
    {
        await ResetDatabase();

        foreach (var scope in serviceScopes)
        {
            scope.Dispose();
        }

        await DataContext.DisposeAsync();

        FakeClock.Reset(SystemClock.Instance.GetCurrentInstant());
    }

    protected T ResolveService<T>()
        where T : notnull
    {
        var scope = fixture.Services.CreateScope();
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
            var options = ResolveService<IOptions<AppOptions>>().Value;
            connectionString = options.ConnectionString;
        }

        if (connection == null)
        {
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
        }

        respawner ??= await Respawner.CreateAsync(connection);

        await respawner.ResetAsync(connection);
    }
}
