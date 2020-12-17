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
using CSharpFunctionalExtensions;
using Xunit;

namespace StudentProgress.ApplicationTests.Groups
{
    [Collection("Database collection")]
    public class CreateIntegrationTests : DatabaseTests
    {
        [Xunit.Fact]
        public async Task Creates_a_new_group()
        {
            var useCase = new Create(CreateUnitOfWork());

            var result = await useCase.HandleAsync(
                new Create.Request { Mnemonic = null, Name = "S3 Leon" });

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value);
        }

        public CreateIntegrationTests(DatabaseFixture fixture) : base(fixture)
        {
        }
    }
}
