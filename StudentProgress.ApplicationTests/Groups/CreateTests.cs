using System;
using StudentProgress.Application.Groups;
using Xunit;

namespace StudentProgress.ApplicationTests.Groups
{
    public class CreateTests
    {
        [Fact]
        public void Cannot_create_group_with_an_empty_name()
        {
            var groupName = GroupName.Create("");

            Assert.Throws<ArgumentNullException>(() => new Group(groupName.Value, null));
        }
    }
}