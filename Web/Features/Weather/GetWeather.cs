namespace Web.Features.Weather;

public static class GetWeather
{
    public record Response(int Temperature, double WindSpeed, string Description);

    public class Endpoint : EndpointWithoutRequest<Response>
    {
        public override void Configure()
        {
            Get($"weather/{nameof(GetWeather)}");
            AllowAnonymous();
        }

        public override Task<Response> ExecuteAsync(CancellationToken ct)
        {
            var rand = new Random();
#pragma warning disable CA5394 // (Do not use insecure randomness) This is only an example
            var temperature = rand.Next(-20, 55);
            var windSpeed = Math.Round(rand.NextDouble() * 100, 2);
            var description = weatherDescriptions[rand.Next(weatherDescriptions.Count)];
#pragma warning restore CA5394

            return Task.FromResult(new Response(temperature, windSpeed, description));
        }

        private readonly List<string> weatherDescriptions =
        [
            "Clear",
            "Cloudy",
            "Fog",
            "Haze",
            "Light Rain",
            "Mostly Cloudy",
            "Overcast",
            "Partly Cloudy",
            "Rain",
            "Rain Showers",
            "Showers",
            "Thunderstorm",
            "Chance of Showers",
            "Chance of Snow",
            "Chance of Storm",
            "Mostly Sunny",
            "Partly Sunny",
            "Scattered Showers",
            "Sunny",
        ];
    }
}
