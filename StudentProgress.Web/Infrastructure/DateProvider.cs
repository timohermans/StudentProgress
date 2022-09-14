using System;
using StudentProgress.Core;

namespace StudentProgress.Web.Infrastructure;

public class DateProvider : IDateProvider
{
    public DateTime Today() => DateTime.Today;
    public DateTime Now() => DateTime.Now;
}