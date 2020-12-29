using System.Linq;
using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.Extensions
{
    public static class GroupExtensions
    {
        public static StudentGroup HasName(this StudentGroup group, string name)
        {
            group.Name.Value.Should().Be(name);
            return group;
        }

        public static StudentGroup HasMnemonic(this StudentGroup group, string mnemonic)
        {
            group.Mnemonic.Should().Be(mnemonic);
            return group;
        }

        public static StudentGroup HasStudent(this StudentGroup group, string studentName)
        {
            group.Students.Any(g => g.Name == studentName).Should().BeTrue();
            return group;
        }
    }
}