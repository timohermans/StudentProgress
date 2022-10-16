using System.Threading;
using FluentAssertions;
using StudentProgress.Core.UseCases;

namespace StudentProgress.CoreTests.UseCases;

[Collection("db")]
public class SettingsSetTests : DatabaseTests
{
    public SettingsSetTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Can_store_new_setting()
    {
        var request = new SettingsSet.Request
        {
            CanvasApiKey = "123-api",
            CanvasApiUrl = "http://test.nl"
        };
        await using var ucDb = Fixture.CreateDbContext();
        var uc = new SettingsSet(ucDb);

        var result = uc.Handle(request, CancellationToken.None);

        result.Result.IsSuccess.Should().BeTrue();
        var settings = await Fixture.DataMother.QueryAllAsync<Setting>();
        settings.Should().HaveCount(2);
        settings[0].Key.Should().Be(Setting.Keys.CanvasApiKey);
        settings[0].Value.Should().Be("123-api");
        settings[1].Key.Should().Be(Setting.Keys.CanvasApiUrl);
        settings[1].Value.Should().Be("http://test.nl");
    }
    
    [Fact]
    public async Task Updates_setting_when_already_exists()
    {
        await Fixture.DataMother.CreateSetting(Setting.Keys.CanvasApiKey, "old-value");
        var request = new SettingsSet.Request
        {
            CanvasApiKey = "123-api",
            CanvasApiUrl = "http://client.nl"
        };
        await using var ucDb = Fixture.CreateDbContext();
        var uc = new SettingsSet(ucDb);

        var result = uc.Handle(request, CancellationToken.None);

        result.Result.IsSuccess.Should().BeTrue();
        var settings = await Fixture.DataMother.QueryAllAsync<Setting>();
        var key = settings.First(s => s.Key == Setting.Keys.CanvasApiKey);
        key.Key.Should().Be(Setting.Keys.CanvasApiKey);
        key.Value.Should().Be("123-api");
    }
}