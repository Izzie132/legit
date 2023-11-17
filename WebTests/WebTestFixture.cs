using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

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
}
