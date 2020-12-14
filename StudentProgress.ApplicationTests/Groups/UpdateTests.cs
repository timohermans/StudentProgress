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
    public class UpdateTests : DatabaseTests
    {
        [Fact]
        public async Task Updates_the_group_with_name()
        {
            await new Create(new UnitOfWork(SessionFactory)).HandleAsync(new Create.Request { Name = "S3 Leon" });
            var useCase = new Update(new UnitOfWork(SessionFactory));

            var result = useCase.HandleAsync(new Update.Request { Id = 1, Name = "S3 Timo", Mnemonic = "Reminder!" });

            var uow = new UnitOfWork(SessionFactory);
            var group = uow.Query<Group>().FirstOrDefault();
            Assert.Equal("S3 Timo", group.Name);
            Assert.Equal("Reminder!", group.Mnemonic);
        }
    }
}
