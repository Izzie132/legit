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

        RemoveServiceIfExists<IClock>(s);
        s.AddSingleton<IClock>(new FakeClock(SystemClock.Instance.GetCurrentInstant()));
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
