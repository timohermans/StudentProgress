using NHibernate.Exceptions;
using StudentProgress.Application;
using StudentProgress.Application.Groups;
using StudentProgress.Application.Groups.UseCases;
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
        public async Task Creates_a_new_group()
        {
            var useCase = new Create(new UnitOfWork(SessionFactory));

            var result = await useCase.HandleAsync(
                new Create.Request { Mnemonic = null, Name = "S3 Leon" });

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value);
        }


        [Fact]
        public async Task Cannot_add_two_group_with_the_same_name()
        {
            await new Create(new UnitOfWork(SessionFactory)).HandleAsync(new Create.Request { Name = "S3 Leon" });
            var useCase = new Create(new UnitOfWork(SessionFactory));

            var result = await useCase.HandleAsync(
                new Create.Request { Mnemonic = "Hallo :)", Name = "S3 Leon" });

            Assert.True(result.IsFailure);
            Assert.Contains("already exists", result.Error);
        }

        [Fact]
        public async Task Cannot_create_group_with_an_empty_name()
        {
            var useCase = new Create(new UnitOfWork(SessionFactory));

            var result = await useCase.HandleAsync(new Create.Request { Name = "" });

            Assert.True(result.IsFailure);
        }
    }
}
