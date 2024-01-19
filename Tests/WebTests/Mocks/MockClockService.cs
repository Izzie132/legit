using NodaTime;
using NodaTime.Testing;
using Web.Services;

namespace WebTests.Mocks;

public class MockClockService : IClockService
{
    public static FakeClock Clock { get; } = new FakeClock(SystemClock.Instance.GetCurrentInstant());

    private IClockService ClockService => new ClockService(Clock);

    public DateTimeZone TimeZone => ClockService.TimeZone;
    public Instant Now => ClockService.Now;
    public LocalDateTime LocalNow => ClockService.LocalNow;

    public Instant ToInstant(LocalDateTime local) => ClockService.ToInstant(local);

    public LocalDateTime ToLocal(Instant instant) => ClockService.ToLocal(instant);

    public static void Reset() => Clock.Reset(SystemClock.Instance.GetCurrentInstant());
}
