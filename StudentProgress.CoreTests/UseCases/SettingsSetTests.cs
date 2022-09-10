using FluentAssertions;
using StudentProgress.Core.UseCases;

namespace StudentProgress.CoreTests.UseCases;

[Collection("db")]
public class SettingsSet : DatabaseTests
{
    public SettingsSet(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Can_store_new_setting()
    {
        var request = new SettingSet.Request
        {
            CanvasApiKey = "123-api"
        };
        await using var ucDb = Fixture.CreateDbContext();
        var uc = new SettingSet(ucDb);

        var result = uc.HandleAsync(request);

        result.Result.IsSuccess.Should().BeTrue();
        var settings = await Fixture.DataMother.QueryAllAsync<Setting>();
        settings.Should().HaveCount(1);
        settings.First().Key.Should().Be(Setting.Keys.CanvasApiKey);
        settings.First().Value.Should().Be("123-api");
    }
    
    [Fact]
    public async Task Updates_setting_when_already_exists()
    {
        await Fixture.DataMother.CreateSetting(Setting.Keys.CanvasApiKey, "old-value");
        var request = new SettingSet.Request
        {
            CanvasApiKey = "123-api"
        };
        await using var ucDb = Fixture.CreateDbContext();
        var uc = new SettingSet(ucDb);

        var result = uc.HandleAsync(request);

        result.Result.IsSuccess.Should().BeTrue();
        var settings = await Fixture.DataMother.QueryAllAsync<Setting>();
        settings.Should().HaveCount(1);
        settings.First().Key.Should().Be(Setting.Keys.CanvasApiKey);
        settings.First().Value.Should().Be("123-api");
    }
}