using System.Threading;
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
        await Fixture.DataMother.CreateSetting(Setting.Keys.CanvasApiKey, "123-canvas-api");
        await Fixture.DataMother.CreateSetting(Setting.Keys.CanvasApiUrl, "http://canvas.nl");
        await using var ucContext = Fixture.CreateDbContext();
        var uc = new SettingsGet(ucContext);

        var result = await uc.Handle(new SettingsGet.Request(), CancellationToken.None);

        result.CanvasApiKey.Should().Be("123-canvas-api");
        result.CanvasUrl.Should().Be("http://canvas.nl");
    }
}