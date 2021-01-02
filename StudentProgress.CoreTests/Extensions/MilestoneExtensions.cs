using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.Extensions
{
    public static class MilestoneExtensions
    {
        public static Milestone HasName(this Milestone milestone, string expected)
        {
            milestone.Name.Should().Be(Name.Create(expected).Value);
            return milestone;
        }
    }
}