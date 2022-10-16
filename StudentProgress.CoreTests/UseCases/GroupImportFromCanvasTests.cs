using System.IO;
using System.Net.Http;
using System.Threading;
using FluentAssertions;
using StudentProgress.Core.UseCases;

namespace StudentProgress.CoreTests.UseCases;

[Collection("db")]
public class GroupImportFromCanvasTests : DatabaseTests
{
    public GroupImportFromCanvasTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Imports_the_selected_canvas_course_section_as_a_group()
    {
        var client = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(1) });
        var config = new CoreTestConfiguration();
        var imageDir = Path.Combine(config.MediaLocation, "images", "avatars");
        if (Directory.Exists(imageDir)) Directory.Delete(imageDir, true);
        var request = new GroupImportFromCanvas.Request
        {
            Name = "S-DB-S2-CMK",
            SectionName = "S2-DB02",
            CanvasId = "12685",
            TermName = "2223nj",
            SectionCanvasId = "1234",
            TermStartsAt = new DateTime(2022, 8, 1),
            TermEndsAt = new DateTime(2023, 3, 27),
            Students = new List<GroupImportFromCanvas.GetCourseDetailsStudent>
            {
                new()
                {
                    Name = "Hermans, Timo T.M.",
                    AvatarUrl = "https://pngimg.com/uploads/apple/apple_PNG12405.png",
                    CanvasId = "1234"
                },
                new()
                {
                    Name = "Luuk",
                    AvatarUrl = null,
                    CanvasId = "1235"
                }
            }
        };
        await using var ucContext = Fixture.CreateDbContext();
        var uc = new GroupImportFromCanvas(ucContext, config, client);

        await uc.Handle(request, CancellationToken.None);

        await using var assertDb = Fixture.CreateDbContext();
        var resultGroup = await assertDb.Groups.Include(g => g.Students).FirstAsync();
        resultGroup.Name.Should().Be((Name)"S-DB-S2-CMK - S2-DB02 - 2223nj");
        resultGroup.Period.StartDate.Should().Be(new DateTime(2022, 8, 29));
        var resultStudents = resultGroup.Students;
        resultStudents.Should().HaveCount(2);
        var timo = resultStudents.First(s => s.Name == "Hermans, Timo T.M.");
        timo.AvatarPath.Should().Be(Path.Combine("images", "avatars", "1234-canvas.png"));
        var luuk = resultStudents.First(s => s.Name == "Luuk");
        luuk.AvatarPath.Should().BeNull();

        Directory.Exists(imageDir).Should().BeTrue();
        var files = Directory.GetFiles(imageDir);
        files.Should().HaveCount(1);
        files.First().Should().Contain("1234-canvas.png");
    }
}