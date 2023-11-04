using System.Net.Http;
using Microsoft.Extensions.Configuration;
using StudentProgress.Web.Lib.CanvasApi;

namespace StudentProgress.CoreTests;

public class IntegrationTests
{
    public CanvasFixture? CanvasFixture { get; set; }
    public DatabaseFixture DatabaseFixture { get; set; }

    public IntegrationTests(DatabaseFixture dbFixture)
    {
        DatabaseFixture = dbFixture;
    }

    public IntegrationTests(CanvasFixture canvasFixture, DatabaseFixture dbFixture) : this(dbFixture)
    {
        CanvasFixture = canvasFixture;
    }
}

public class CanvasFixture
{
    public ICanvasClient Client { get; }

    public CanvasFixture()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.real.json", false, false)
            .Build();
        var client = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromHours(1) });
        Client = new CanvasClient(client, new InfraConfigProvider(configuration));
    }
}

[CollectionDefinition("integration")]
public class IntegrationCollectionDefinition : ICollectionFixture<CanvasFixture>, ICollectionFixture<DatabaseFixture>
{
}

public class InfraConfigProvider : ICanvasApiConfig
{
    private readonly IConfiguration _config;

    public InfraConfigProvider(IConfiguration config)
    {
        _config = config;
    }

    public string? CanvasApiKey => _config.GetValue<string>("canvas:key");
    public string? CanvasApiUrl => _config.GetValue<string>("canvas:url");
    public Task<bool> CanUseCanvasApiAsync() => Task.FromResult(true);
}