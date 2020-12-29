using FluentAssertions;
using StudentProgress.Core.Entities;
using Xunit;

namespace StudentProgress.CoreTests.Models
{
    public class Group
    {
        [Fact]
        public void Name_is_not_empty()
        {
            var groupName = Name.Create("");

            groupName.IsFailure.Should().BeTrue();
        }
    }
}