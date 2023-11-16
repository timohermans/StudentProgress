using StudentProgress.Core.Models;
using StudentProgress.Web.Pages.People;

namespace StudentProgress.CoreTests.Pages.People.Parts;

[Collection("integration")]
public class SearchTests(DatabaseFixture fixture) : IntegrationTests(fixture)
{
    [Fact]
    public async Task Searches_on_student_name_with_case_insensitivity()
    {
        using (var db1 = DatabaseFixture.CreateWebContext())
        {
            var timo = new Person { FirstName = "Timo", LastName = "", AvatarPath = "123.png" };
            await db1.People.AddAsync(timo);
            await db1.Adventures.AddAsync(new Core.Models.Adventure
            {
                Name = "semester 1",
                People = new List<Person> { timo, new() { FirstName = "Leon", LastName = "" } },
                DateStart = DateTime.Now
            });
            await db1.Adventures.AddAsync(new Core.Models.Adventure
                { Name = "semester 2", People = new List<Person> { timo }, DateStart = DateTime.Now });
            await db1.Adventures.AddAsync(new Core.Models.Adventure
            {
                Name = "semester 3",
                People = new List<Person> { new() { FirstName = "Simon", LastName = "" } },
                DateStart = DateTime.Now
            });
            await db1.SaveChangesAsync();
        }

        await using var db = DatabaseFixture.CreateWebContext();
        var page = new SearchModel(db);

        await page.OnGet("timo");

        page.People.Should().HaveCount(1);
        page.People.First().FirstName.Should().Be("Timo");
        page.People.First().AvatarPath.Should().Be("123.png");
        page.People.First().Adventures.Should().HaveCount(2);
        page.People.First().Adventures.Select(g => g.Name).Should().Contain("semester 1");
        page.People.First().Adventures.Select(g => g.Name).Should().Contain("semester 2");
    }
}