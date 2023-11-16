namespace StudentProgress.Core.Infrastructure;

public interface IDateProvider
{
    DateTime Today();
    DateTime Now();
}