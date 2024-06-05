using System.Net;
using Web.Features.Weather;

namespace WebTests.Features.Weather;

public class GetWeatherTests(WebTestFixture f) : BaseWebTest(f)
{
    [Fact]
    public async Task ValidRequest_HasValidData()
    {
        var (httpResponseMessage, response) = await Client.GETAsync<GetWeather.Endpoint, GetWeather.Response>();

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.True(response.WindSpeed >= 0);
        Assert.True(response.Temperature >= -273.15);
    }
}
