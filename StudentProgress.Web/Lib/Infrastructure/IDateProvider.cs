using System;

namespace StudentProgress.Web.Lib.Infrastructure;

public interface IDateProvider
{
    DateTime Today();
    DateTime Now();
}