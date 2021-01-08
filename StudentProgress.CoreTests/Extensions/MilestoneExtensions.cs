using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.Extensions
{
    public static class MilestoneExtensions
    {
        public static Milestone HasArtefact(this Milestone milestone, string expected)
        {
            milestone.Artefact.Should().Be(Name.Create(expected).Value);
            return milestone;
        }

        public static Milestone HasLearningOutcome(this Milestone milestone, string expected)
        {
            milestone.LearningOutcome.Should().Be(Name.Create(expected).Value);
            return milestone;
        }

    }
}