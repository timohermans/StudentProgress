using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using StudentProgress.Core.Models;

namespace StudentProgress.CoreTests.Pages.Adventure;

[Collection("integration")]
public class OnDeleteRemovePersonTests(DatabaseFixture dbFixture, WebApplicationFactory<Program> factory) : IntegrationTests(
    dbFixture, factory), IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Cannot_remove_person_on_nonexistent_adventure()
    {
        using var client = await CreateAuthenticatedAppClientAsync();
        var timo = new Person
        {
            FirstName = "Timo",
            LastName = "Hermans",
        };

        await using var dbSetup = DatabaseFixture.CreateWebContext();
        await dbSetup.People.AddAsync(timo);
        await dbSetup.SaveChangesAsync();

        var response = await client.DeleteAsync($"/adventure/1?personid={timo.Id}&handler=removeperson");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Removes_person_from_adventure()
    {
        using var client = await CreateAuthenticatedAppClientAsync();
        var adventure = new Core.Models.Adventure
        {
            Name = "S3CB",
            DateStart = DateTime.Today,
        };
        var timo = new Person
        {
            FirstName = "Timo",
            LastName = "Hermans",
        };
        var allert = new Person
        {
            FirstName = "Allert",
            LastName = "van der Wal",
        };
        adventure.People.Add(timo);
        adventure.People.Add(allert);

        await using var dbSetup = DatabaseFixture.CreateWebContext();
        await dbSetup.Adventures.AddAsync(adventure);
        await dbSetup.SaveChangesAsync();

        // act
        var response = await client.DeleteAsync($"/adventure/{adventure.Id}?personid={allert.Id}&handler=removeperson");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.SeeOther);
        await using var assertDb = DatabaseFixture.CreateWebContext();
        var dbAdventure = await assertDb.Adventures
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Id == adventure.Id);

        dbAdventure.Should().NotBeNull();
        dbAdventure!.People.Should().HaveCount(1);
        dbAdventure.People.First().Name.Should().Be(timo.Name);
    }
}