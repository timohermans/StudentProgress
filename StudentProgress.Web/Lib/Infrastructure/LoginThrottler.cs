namespace StudentProgress.Web.Lib.Infrastructure;

public class LoginThrottler
{
    private readonly IDateProvider _dateProvider;
    private readonly int _lockoutMax = 5;
    private DateTime _tryAgainTime;
    private int _lockoutCount = 0;

    public LoginThrottler(IDateProvider dateProvider)
    {
        _dateProvider = dateProvider;
        _tryAgainTime = dateProvider.Now();
    }

    public void Throttle()
    {
        if (_lockoutCount >= _lockoutMax && _dateProvider.Now() < _tryAgainTime)
        {
            throw new InvalidOperationException("Cannot add another lockout when already locked out");
        }

        _lockoutCount++;

        if (_lockoutCount < _lockoutMax)
        {
            _tryAgainTime = _dateProvider.Now();
        }
        else
        {
            var attemptsTooMuch = _lockoutCount - _lockoutMax + 1; // start with one, not 0
            var secondsToWaitBeforeRetry = Math.Pow(4, attemptsTooMuch);
            _tryAgainTime = _dateProvider.Now().AddSeconds(secondsToWaitBeforeRetry);
        }
    }

    public int GetSecondsLeftToTryAgain()
    {
        if (_lockoutCount < _lockoutMax) return 0;
        var secondsToRetry = (_tryAgainTime - _dateProvider.Now()).TotalSeconds;

        return secondsToRetry < 0 ? 0 : (int)secondsToRetry;
    }

    public void Reset()
    {
        _lockoutCount = 0;
        _tryAgainTime = _dateProvider.Now();
    }
}