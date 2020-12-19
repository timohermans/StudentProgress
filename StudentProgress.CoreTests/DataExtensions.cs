using CSharpFunctionalExtensions;
using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests
{
    public static class DataExtensions
    {
        public static T ShouldExist<T>(this T entity)
        {
            entity.Should().NotBe(null);
            return entity;
        }
    }
}