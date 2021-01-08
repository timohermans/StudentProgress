using FluentAssertions;
using StudentProgress.Core.Entities;
using Xunit;

namespace StudentProgress.CoreTests.Models
{
    public class GroupTests
    {
        [Fact]
        public void Name_is_not_empty()
        {
            var groupName = Name.Create("");

            groupName.IsFailure.Should().BeTrue();
        }
    }
}