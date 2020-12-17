using NHibernate.Linq;
using StudentProgress.Application;
using StudentProgress.Application.Groups.UseCases;
using StudentProgress.Application.Students;
using StudentProgress.Application.Students.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StudentProgress.ApplicationTests.Students
{
    [Collection("Database collection")]
    public class AddToGroupTests : DatabaseTests
    {
        public AddToGroupTests(DatabaseFixture fixture) : base(fixture)
        {
        }
        
        [Xunit.Fact]
        public async Task Adds_a_new_student_to_a_group()
        {
            var uow = CreateUnitOfWork();
            var groupId = await new Create(CreateUnitOfWork()).HandleAsync(new Create.Request { Mnemonic = null, Name = "S3 Leon" });
            var useCase = new AddToGroup(CreateUnitOfWork());

            var result = await useCase.HandleAsync(new AddToGroup.Request { GroupId = groupId.Value, Name = "Timo" });

            Assert.True(result.IsSuccess);
            var student = await uow.Query<Student>().FirstAsync();
            Assert.Equal("Timo", student.Name);
            Assert.Equal(1, student.Id);
            Assert.Equal("S3 Leon", student.Groups.FirstOrDefault().Name);
        }

        [Fact]
        public async Task Cannot_add_the_same_student_to_a_group()
        {
            var groupId = await new Create(CreateUnitOfWork()).HandleAsync(new Create.Request { Mnemonic = null, Name = "S3 Leon" });
            await new AddToGroup(CreateUnitOfWork()).HandleAsync(new AddToGroup.Request { GroupId = groupId.Value, Name = "Timo" });
            var useCase = new AddToGroup(CreateUnitOfWork());
            
            var result = await useCase.HandleAsync(new AddToGroup.Request { GroupId = groupId.Value, Name = "Timo" });

            Assert.True(result.IsFailure);
            Assert.Contains("already", result.Error);
        }
    }
}
