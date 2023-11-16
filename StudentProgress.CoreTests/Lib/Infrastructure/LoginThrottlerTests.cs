using StudentProgress.Core.Infrastructure;

namespace StudentProgress.CoreTests.Lib.Infrastructure;

public class DummyDateProvider : IDateProvider
{
    private DateTime _date;

    public DummyDateProvider(DateTime date)
    {
        _date = date;
    }

    public DateTime Today()
    {
        return _date.Date;
    }

    public DateTime Now()
    {
        return _date;
    }

    public void Advance(TimeSpan span)
    {
        _date = _date.Add(span);
    }
}

public class LoginThrottlerTests
{
    [Fact]
    public void Can_add_lockout_after_certain_time()
    {
        var now = new DateTime(2023, 1, 1);
        var dateProvider = new DummyDateProvider(now);
        var throttler = new LoginThrottler(dateProvider);
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();

        throttler.Throttle();
        throttler.GetSecondsLeftToTryAgain().Should().Be(4);

        dateProvider.Advance(new TimeSpan(0, 0, 20));
        throttler.GetSecondsLeftToTryAgain().Should().Be(0);

        throttler.Throttle();
        throttler.GetSecondsLeftToTryAgain().Should().Be(16);
        dateProvider.Advance(new TimeSpan(0, 0, 16));

        throttler.Throttle();
        throttler.GetSecondsLeftToTryAgain().Should().Be(64);
        dateProvider.Advance(new TimeSpan(0, 0, 64));

        throttler.Reset();
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();

        throttler.Throttle();
        throttler.GetSecondsLeftToTryAgain().Should().Be(4);
    }

    [Fact]
    public void Cannot_add_lockout_when_locked_out()
    {
        var throttler = new LoginThrottler(new DummyDateProvider(new DateTime(2023, 1, 1)));
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();
        throttler.Throttle();

        Assert.Throws<InvalidOperationException>(() => { throttler.Throttle(); });
    }
}