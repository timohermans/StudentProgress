namespace StudentProgress.Core.Infrastructure;

public class DateProvider : IDateProvider
{
    public DateTime Today() => DateTime.Today;
    public DateTime Now() => DateTime.Now;
}