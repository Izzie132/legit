using System.Net;
using Web.Features.Weather;

namespace IntegrationTests.Features.Weather;

public class GetWeatherTests : BaseWebTest
{
    public GetWeatherTests(WebTestFixture f, ITestOutputHelper o)
        : base(f, o) { }

    [Fact]
    public async Task ValidRequest_HasValidData()
    {
        var (rsp, res) = await Client.GETAsync<GetWeather.Endpoint, GetWeather.Response>();

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.True(res.WindSpeed >= 0);
        Assert.True(res.Temperature >= -273.15);
    }
}
