using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Testing;
using Web;

namespace WebTests;

public class WebTestFixture(IMessageSink s) : TestFixture<Program>(s)
{
    // This is used in AddSingleton, so has to be static if we want to be able to change it in the tests
    // because ConfigureServices is only ever called once, but a new WebTestFixture is created for each test suite
    public static readonly FakeClock FakeClock = new(SystemClock.Instance.GetCurrentInstant());

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        base.ConfigureApp(a);

        a.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddJsonFile("appsettings.Testing.json");
        });
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        base.ConfigureServices(s);

        RemoveServiceIfExists<ZonedClock>(s);
        s.AddSingleton(
            FakeClock.InZone(
                DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London") ?? throw new Exception("Time zone not found")
            )
        );
    }

    private void RemoveServiceIfExists<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
}
