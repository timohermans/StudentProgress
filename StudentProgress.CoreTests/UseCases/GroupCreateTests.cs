﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class GroupCreateTests : DatabaseTests
    {
        public GroupCreateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        private async Task<Result<int>> ActUseCase(Func<GroupCreate, Task<Result<int>>> action)
        {
            await using var dbContext = Fixture.CreateDbContext();
            var useCase = new GroupCreate(dbContext);
            return await action(useCase);
        }

        [Fact]
        public async Task Can_create_a_group()
        {
            var request = new GroupCreate.Request
            {
                Name = "S3 - Leon",
                Mnemonic = null,
                StartPeriod = new DateTime(2020, 9, 1)
            };

            var result = await ActUseCase(useCase => useCase.Handle(request, CancellationToken.None));

            Assert.True(result.IsSuccess);
            await using var assertDb = new ProgressContext(Fixture.ContextOptions);
            var group = assertDb.Groups.FirstOrDefault();
            assertDb.Groups.Should().HaveCount(1);
            group.Should().NotBeNull();
            group!.Name.Value.Should().Be("S3 - Leon");
            group!.Mnemonic.Should().Be(null);
        }

        [Fact]
        public async Task Cannot_create_a_duplicate_group_on_the_same_period()
        {
            Fixture.DataMother.CreateGroup("S3-Leon");
            var request = new GroupCreate.Request
            {
                Name = "S3-Leon",
                Mnemonic = "Dit is een test",
                StartDate = new DateTime(2020, 9, 1)
            };

            var result = await ActUseCase(useCase => useCase.Handle(request, CancellationToken.None));

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("already exists");
        }
    }
}