﻿using StudentProgress.Web.Models;
using StudentProgress.Web.Pages.People.Parts;

namespace StudentProgress.CoreTests.Pages.People.Parts
{
    [Collection("db")]
    public class SearchTests : DatabaseTests
    {
        public SearchTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Searches_on_student_name_with_case_insensitivity()
        {
            using (var db1 = Fixture.CreateWebContext())
            {
                var timo = new Person { Name = "Timo", AvatarPath = "123.png" };
                await db1.People.AddAsync(timo);
                await db1.Adventures.AddAsync(new Adventure { Name = "semester 1", People = new List<Person> { timo, new Person { Name = "Leon" } }, DateStart = DateTime.Now });
                await db1.Adventures.AddAsync(new Adventure { Name = "semester 2", People = new List<Person> { timo }, DateStart = DateTime.Now });
                await db1.Adventures.AddAsync(new Adventure { Name = "semester 3", People = new List<Person> { new Person { Name = "Simon" } }, DateStart = DateTime.Now });
                await db1.SaveChangesAsync();
            }

            await using var db = Fixture.CreateWebContext();
            var page = new SearchModel(db);

            await page.OnGet("timo");

            page.People.Should().HaveCount(1);
            page.People.First().Name.Should().Be("Timo");
            page.People.First().AvatarPath.Should().Be("123.png");
            page.People.First().Adventures.Should().HaveCount(2);
            page.People.First().Adventures.Select(g => g.Name).Should().Contain("semester 1");
            page.People.First().Adventures.Select(g => g.Name).Should().Contain("semester 2");
        }
    }
}