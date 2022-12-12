using System.Threading;
using FluentAssertions;
using StudentProgress.Core.UseCases.Canvas.Courses;

namespace StudentProgress.CoreTests.UseCases.Canvas.Courses;

[Collection("canvas")]
public class GetCourseDetailsTests : CanvasTests
{
    public GetCourseDetailsTests(CanvasFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Gets_course_details_of_semester_2_2223nj()
    {
        var uc = new GetCourseDetailsUseCase(Fixture.Client);

        var result = (await uc.Handle(new GetCourseDetailsUseCase.Command("12685"), CancellationToken.None)).Courses.ToList();

        result.Should().NotBeNull();
        result.Should().HaveCount(6);
        var class2 = result.FirstOrDefault(c => c.SectionName == "S2-DB02");
        class2.Should().NotBeNull();
        class2!.Students.Should().HaveCount(23);
        var student1 = class2.Students.FirstOrDefault();
        student1.Should().NotBeNull();
        student1!.Name.Should().Contain("Mees M.");
        student1.AvatarUrl.Should().Contain("1048629/6sNS2LrBPDkn");
        student1.CanvasId.Should().Be("19797");
    }
}