using StudentProgress.Application;
using StudentProgress.Application.Groups;
using StudentProgress.Application.Groups.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StudentProgress.ApplicationTests.Groups
{
    [Collection("Database collection")]
    public class UpdateTests : DatabaseTests
    {
        [Fact]
        public async Task Updates_the_group_with_name()
        {
            await new Create(CreateUnitOfWork()).HandleAsync(new Create.Request { Name = "S3 Leon" });
            var useCase = new Update(CreateUnitOfWork());

            var result = await useCase.HandleAsync(new Update.Request { Id = 1, Name = "S3 Timo", Mnemonic = "Reminder!" });

            Assert.True(result.IsSuccess);
            var uow = CreateUnitOfWork();
            var group = uow.Query<Group>().FirstOrDefault();
            Assert.Equal("S3 Timo", group.Name);
            Assert.Equal("Reminder!", group.Mnemonic);
        }

        public UpdateTests(DatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
