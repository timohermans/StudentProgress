using StudentProgress.Application.Groups;
using StudentProgress.Application.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StudentProgress.ApplicationTests.Groups
{
    public class CreateTests : DatabaseTests
    {
        [Xunit.Fact]
        public void Creates_a_new_group()
        {
            using var transaction = Session.BeginTransaction();

            var group = new Group("S3 Leon", null);

            Session.SaveOrUpdate(group);

            transaction.Commit();
        }

        [Xunit.Fact]
        public void Adds_a_new_student_to_a_group()
        {
            using var transaction = Session.BeginTransaction();
            var group = new Group("S3 Leon", null);
            group.AddStudent(new Student("Timo Hermans"));

            Session.SaveOrUpdate(group);
            transaction.Commit();

            Assert.Equal(1, group.Id);
            Assert.Single(group.Students);
        }
    }
}
