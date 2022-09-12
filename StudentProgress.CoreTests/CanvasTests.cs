using System.Net.Http;
using Microsoft.Extensions.Configuration;
using StudentProgress.Core.CanvasApi;

namespace StudentProgress.CoreTests;

public class CanvasTests
{
    public CanvasFixture Fixture { get; set; }

    public CanvasTests(CanvasFixture fixture) => Fixture = fixture;
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

[CollectionDefinition("canvas")]
public class CanvasCollectionDefinition : ICollectionFixture<CanvasFixture> {}

public class InfraConfigProvider : ICanvasApiConfig
{
    private readonly IConfiguration _config;

    public InfraConfigProvider(IConfiguration config)
    {
        _config = config;
    }

    public string CanvasApiKey => _config.GetValue<string>("canvas:key");
    public string? CanvasApiUrl => _config.GetValue<string>("canvas:url");
}
