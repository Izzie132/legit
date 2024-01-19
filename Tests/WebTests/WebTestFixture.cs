using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web;
using Web.Services;
using WebTests.Mocks;

namespace WebTests;

public class WebTestFixture : TestFixture<Program>
{
    public WebTestFixture(IMessageSink s)
        : base(s) { }

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

        RemoveServiceIfExists<IClockService>(s);
        s.AddScoped<IClockService, MockClockService>();
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
