using System.Net.Http;
using System.Threading;
using FluentAssertions;
using StudentProgress.Web.Pages.Canvas.Courses;

namespace StudentProgress.CoreTests.Pages.Canvas.Courses;

[Collection("canvas")]
public class DetailsOnGet : CanvasTests
{
    private readonly DatabaseFixture _databaseFixture;

    public DetailsOnGet(CanvasFixture canvasFixture, DatabaseFixture databaseFixture) : base(canvasFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task Gets_course_details_of_semester_2_2223nj()
    {
        var client = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(1) });
        var config = new CoreTestConfiguration();
        await using var context = _databaseFixture.CreateWebContext();
        var page = new Details(CanvasFixture.Client, context, config, client);

        await page.OnGetAsync("12685", CancellationToken.None);

        var result = page.Semesters;
        result.Should().NotBeNull();
        result.Should().HaveCount(6);
        var class2 = result.FirstOrDefault(c => c.SectionName == "S2-DB02");
        class2.Should().NotBeNull();
        class2!.Students.Should().HaveCount(22);
        var student1 = class2.Students.FirstOrDefault();
        student1.Should().NotBeNull();
        student1!.Name.Should().Contain("Mees M.");
        student1.AvatarUrl.Should().Contain("1048629/6sNS2LrBPDkn");
        student1.CanvasId.Should().Be("19797");
    }
}