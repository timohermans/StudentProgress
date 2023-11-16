using System.Threading;
using Index = StudentProgress.Web.Pages.Canvas.Courses.Index;

namespace StudentProgress.CoreTests.Pages.Canvas.Courses;

[Collection("integration")]
public class IndexOnGet(CanvasFixture fixture, DatabaseFixture dbFixture) : IntegrationTests(fixture, dbFixture)
{

    // so it could be this test fails eventually, with three possibilities:
    // 1) the api token has expired -> get a new one from canvas and put it in appsettings.real.json
    // 2) the url to the canvas api has changed -> get the new and put it in appsettings.real.json
    // 3) the data retrieved has changed or has been removed -> debug and fix
    // of course all of this can also break when they decide to shut everything off or change the whole api
    [Fact]
    public async Task Gets_all_courses()
    {
        var page = new Index(CanvasFixture!.Client);

        await page.OnGetAsync(new CancellationToken());

        var result = page.Courses.ToList();
        result.Should().NotBeNull();
        var sem1 = result.First(s => s.Id == "12461");
        sem1.Should().NotBeNull();
        sem1.Name.Should().Be("P-SEM1-CB-CMK");
        sem1.Term!.Name.Should().Be("2223nj");
        sem1.Term.StartAt.Should().NotBeNull();
        sem1.Term.EndAt.Should().NotBeNull();
        // sem1.Term.StartAt.Value.Should().Be(new DateTime(2022, 8, 1));
        // sem1.Term.EndAt.Value.Should().Be(new DateTime(2023, 3, 27));

        var sem2 = result.First(s => s.Id == "12685");
        sem2.Should().NotBeNull();
        sem2.Name.Should().Be("S-DB-S2-CMK");
        sem2.Term!.Name.Should().Be("2223nj");
        sem2.Term.StartAt.Should().NotBeNull();
        sem2.Term.EndAt.Should().NotBeNull();
        // sem2.Term.StartAt.Value.AsUtc().Should().Be(new DateTime(2022, 8, 1));
        // sem2.Term.EndAt.Value.AsUtc().Should().Be(new DateTime(2023, 3, 27));
    }
}