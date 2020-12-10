using System;
using System.Linq;
using StudentProgress.Core.Entities;
using Xunit;

namespace StudentProgress.Tests
{
    public class SqliteDbContextTest : DatabaseTests
    {
        [Fact]
        public void DbContext_CreatingASimpleEntity_SuccessfullyInserts()
        {
            using var dbContext = new ProgressContext(ContextOptions);
            dbContext.StudentGroup.Add(new StudentGroup("S3 Leon"));
            dbContext.SaveChanges();

            var group = dbContext.StudentGroup.FirstOrDefault();
            
            Assert.NotNull(group);
            Assert.NotEqual(0, group.Id);
        }
    }
}