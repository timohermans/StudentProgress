namespace StudentProgress.Core;

public interface IDateProvider
{
    DateTime Today();
    DateTime Now();
}