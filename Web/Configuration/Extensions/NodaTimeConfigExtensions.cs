using NodaTime;
using NodaTime.Extensions;

namespace Web.Configuration.Extensions;

public static class NodaTimeConfigExtensions
{
    public static void ConfigureNodaTime(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton<ZonedClock>(sp =>
        {
            var clock = sp.GetRequiredService<IClock>();
            return clock.InZone(
                DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London") ?? throw new Exception("Time zone not found")
            );
        });
    }
}
