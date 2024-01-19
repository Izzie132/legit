using NodaTime;
using NodaTime.TimeZones;

namespace Web.Services;

public interface IClockService
{
    DateTimeZone TimeZone { get; }

    Instant Now { get; }

    LocalDateTime LocalNow { get; }

    Instant ToInstant(LocalDateTime local);

    LocalDateTime ToLocal(Instant instant);
}

public class ClockService : IClockService
{
    protected readonly IClock clock;

    public DateTimeZone TimeZone { get; private set; }

    public ClockService()
        : this(SystemClock.Instance) { }

    public ClockService(IClock clock)
    {
        this.clock = clock;
        TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London") ?? throw new Exception("Time zone not found");
    }

    public Instant Now => clock.GetCurrentInstant();

    public LocalDateTime LocalNow => Now.InZone(TimeZone).LocalDateTime;

    public Instant ToInstant(LocalDateTime local) => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

    public LocalDateTime ToLocal(Instant instant) => instant.InZone(TimeZone).LocalDateTime;
}
