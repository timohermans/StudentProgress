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
                    Name = "Timo",
                    AvatarUrl = "http://avatar1.net",
                    CanvasId = "1"
                },
                new()
                {
                    Name = "Luuk",
                    AvatarUrl = null,
                    CanvasId = "2"
                }
            }
        };
        await using var ucContext = Fixture.CreateDbContext();
        var uc = new GroupImportFromCanvas(ucContext);

        await uc.HandleAsync(request);

        await using var assertDb = Fixture.CreateDbContext();
        var resultGroup = await assertDb.Groups.Include(g => g.Students).FirstAsync();
        resultGroup.Name.Should().Be((Name)"S-DB-S2-CMK");
        resultGroup.Period.StartDate.Should().Be(new DateTime(2022, 8, 29));
        var resultStudents = resultGroup.Students;
        resultStudents.Should().HaveCount(2);
        var timo = resultStudents.First(s => s.Name == "Timo");
        timo.AvatarPath.Should().Be($"{timo.Id}.png");
        var luuk = resultStudents.First(s => s.Name == "Luuk");
        luuk.AvatarPath.Should().BeNull();
    }
}