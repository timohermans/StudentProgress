using System.IO;
using System.Net.Http;
using System.Threading;
using StudentProgress.Web.Pages.Canvas.Courses;

namespace StudentProgress.CoreTests.Pages.Canvas.Courses;

[Collection("integration")]
public class DetailsOnPost(CanvasFixture fixture, DatabaseFixture dbFixture) : IntegrationTests(fixture, dbFixture)
{
    [Fact]
    public async Task Imports_the_selected_canvas_course_section_as_a_group()
    {
        await DatabaseFixture.WebDataMother.CreateAdventure(new Core.Models.Adventure
        {
            Name = "Random other adventure",
            DateStart = new DateTime(2022, 8, 1),
        });
        var client = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(1) });
        var config = new CoreTestConfiguration();
        var imageDir = Path.Combine(config.MediaLocation, "images", "avatars");
        if (Directory.Exists(imageDir)) Directory.Delete(imageDir, true);
        var request = new Details.Request
        {
            Name = "S-DB-S2-CMK",
            SectionName = "S2-DB02",
            CanvasId = "12685",
            TermName = "2223nj",
            SectionCanvasId = "1234",
            TermStartsAt = new DateTime(2022, 8, 1),
            TermEndsAt = new DateTime(2023, 3, 27),
            Students = [
                new()
                {
                    Name = "Hermans, Timo T.M.",
                    AvatarUrl = "https://pngimg.com/uploads/apple/apple_PNG12405.png",
                    CanvasId = "1234"
                },
                new()
                {
                    Name = "Hermans, Luuk",
                    AvatarUrl = null,
                    CanvasId = "1235"
                }
            ]
        };
        await using var ucContext = DatabaseFixture.CreateWebContext();
        var page = new Details(CanvasFixture!.Client, ucContext, new CoreTestConfiguration(), client)
        {
            Semester = request
        };

        // act
        await page.OnPostAsync(CancellationToken.None);

        await using var assertDb = DatabaseFixture.CreateWebContext();
        var resultGroup = await assertDb.Adventures.Include(g => g.People)
            .FirstAsync(g => g.Name == "S-DB-S2-CMK - S2-DB02");
        resultGroup.Should().NotBeNull();
        resultGroup!.DateStart.Should().Be(new DateTime(2022, 8, 1));
        var resultPeople = resultGroup.People;
        resultPeople.Should().HaveCount(2);
        var timo = resultPeople.First(s => s.FirstName == "Timo");
        timo.AvatarPath.Should().Be(Path.Combine("images", "avatars", "1234-canvas.png"));
        timo.LastName.Should().Be("Hermans");
        timo.Initials.Should().Be("T.M.");
        var luuk = resultPeople.First(s => s.FirstName == "Luuk");
        luuk.LastName.Should().Be("Hermans");
        luuk.Initials.Should().BeNull();
        luuk.AvatarPath.Should().BeNull();

        Directory.Exists(imageDir).Should().BeTrue();
        var files = Directory.GetFiles(imageDir);
        files.Should().HaveCount(1);
        files.First().Should().Contain("1234-canvas.png");
    }
}