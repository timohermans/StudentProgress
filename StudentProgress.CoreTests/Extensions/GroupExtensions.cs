using System.Linq;
using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.Extensions
{
    public static class GroupExtensions
    {
        public static Group HasName(this Group group, string name)
        {
            group.Name.Value.Should().Be(name);
            return group;
        }

        public static Group HasMnemonic(this Group group, string mnemonic)
        {
            group.Mnemonic.Should().Be(mnemonic);
            return group;
        }

        public static Group HasStudent(this Group group, string studentName)
        {
            group.Students.Any(g => g.Name == studentName).Should().BeTrue();
            return group;
        }
    }
}