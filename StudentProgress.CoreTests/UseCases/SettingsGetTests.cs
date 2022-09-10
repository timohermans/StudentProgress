using FluentAssertions;
using StudentProgress.Core.UseCases;

namespace StudentProgress.CoreTests.UseCases;

[Collection("db")]
public class SettingsGetTests : DatabaseTests
{
    public SettingsGetTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Gets_the_settings()
    {
        await Fixture.DataMother.CreateSetting("canvasApiKey", "123-canvas-api");
        await using var ucContext = Fixture.CreateDbContext();
        var uc = new SettingsGet(ucContext);

        var result = await uc.HandleAsync();

        result.CanvasApiKey.Should().Be("123-canvas-api");
    }
}