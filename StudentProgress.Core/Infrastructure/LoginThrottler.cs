namespace StudentProgress.Core.Infrastructure;

public class LoginThrottler(IDateProvider dateProvider)
{
    private readonly int _lockoutMax = 5;
    private DateTime _tryAgainTime = dateProvider.Now();
    private int _lockoutCount = 0;

    public void Throttle()
    {
        if (_lockoutCount >= _lockoutMax && dateProvider.Now() < _tryAgainTime)
        {
            throw new InvalidOperationException("Cannot add another lockout when already locked out");
        }

        _lockoutCount++;

        if (_lockoutCount < _lockoutMax)
        {
            _tryAgainTime = dateProvider.Now();
        }
        else
        {
            var attemptsTooMuch = _lockoutCount - _lockoutMax + 1; // start with one, not 0
            var secondsToWaitBeforeRetry = Math.Pow(4, attemptsTooMuch);
            _tryAgainTime = dateProvider.Now().AddSeconds(secondsToWaitBeforeRetry);
        }
    }

    public int GetSecondsLeftToTryAgain()
    {
        if (_lockoutCount < _lockoutMax) return 0;
        var secondsToRetry = (_tryAgainTime - dateProvider.Now()).TotalSeconds;

        return secondsToRetry < 0 ? 0 : (int)secondsToRetry;
    }

    public void Reset()
    {
        _lockoutCount = 0;
        _tryAgainTime = dateProvider.Now();
    }
}