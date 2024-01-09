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
        var (httpResponseMessage, response) = await Client.GETAsync<GetWeather.Endpoint, GetWeather.Response>();

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.True(response.WindSpeed >= 0);
        Assert.True(response.Temperature >= -273.15);
    }
}
