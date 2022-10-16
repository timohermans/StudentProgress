﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class SearchTests : DatabaseTests
    {
        public SearchTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Searches_on_student_name()
        {
            var timo = new TestStudent("Timo", null, "123.png");
            Fixture.DataMother.CreateGroup("semester 1", students: new[] {timo, new TestStudent("Leon")});
            Fixture.DataMother.CreateGroup("semester 2", students: new[] {timo});
            Fixture.DataMother.CreateGroup("semester 3", students: new[] {new TestStudent("Simon")});
            await using var ucContext = Fixture.CreateDbContext();
            var useCase = new SearchStudents(ucContext);

            var response = await useCase.Handle(new SearchStudents.Query {SearchTerm = "timo"});

            response.Should().HaveCount(1);
            response.First().Name.Should().Be("Timo");
            response.First().AvatarPath.Should().Be("123.png");
            response.First().Groups.Should().HaveCount(2);
            response.First().Groups.Select(g => g.Name).Should().Contain("semester 1");
            response.First().Groups.Select(g => g.Name).Should().Contain("semester 2");
        }
    }
}