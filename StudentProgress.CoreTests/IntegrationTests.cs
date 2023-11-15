﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using AngleSharp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentProgress.Core.Entities;
using StudentProgress.Web.Lib.CanvasApi;
using StudentProgress.Web.Lib.Data;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace StudentProgress.CoreTests;

public class IntegrationTests
{
    private WebApplicationFactory<Program>? _factory;
    public CanvasFixture? CanvasFixture { get; set; }
    public DatabaseFixture DatabaseFixture { get; set; }

    public IntegrationTests(DatabaseFixture dbFixture)
    {
        DatabaseFixture = dbFixture;
    }

    public IntegrationTests(DatabaseFixture dbFixture, WebApplicationFactory<Program> factory) : this(dbFixture)
    {
        _factory = factory;
    }

    public IntegrationTests(CanvasFixture canvasFixture, DatabaseFixture dbFixture) : this(dbFixture)
    {
        CanvasFixture = canvasFixture;
    }

    /// <summary>
    /// 
    /// inspired by https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
    /// </summary>
    public async Task<HttpClient> CreateAuthenticatedAppClientAsync()
    {
        if (_factory == null) throw new ArgumentNullException();
        var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", options => { });

                    using var provider = services.BuildServiceProvider();
                    using var db = provider.GetRequiredService<WebContext>();
                    using var oldDb = provider.GetRequiredService<ProgressContext>();

                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                    oldDb.Database.EnsureDeleted();
                    oldDb.Database.Migrate();
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false, // this makes sure you will not get a 200 at the /login page automatically
            });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");

        var response = await client.GetAsync("/adventures");
        var xsrfToken = await GetXsrfTokenForCud(await response.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Add("X-Xsrf-Token", xsrfToken);

        return client ?? throw new NullReferenceException("Something went wrong creating the client");
    }

    public async Task<string> GetXsrfTokenForCud(string htmlContent)
    {
        var browser = BrowsingContext.New(Configuration.Default);
        var document = await browser.OpenAsync(req => req.Content(htmlContent));
        var body = document.QuerySelector("body") ??
                   throw new InvalidOperationException("Unable to get /adventurs html");
        var headerJson = body.Attributes.GetNamedItem("hx-headers")?.Value ??
                         throw new NullReferenceException("Unable to get token from body");
        Dictionary<string, object> htmxHeaderConfig =
            JsonSerializer.Deserialize<Dictionary<string, object>>(headerJson) ??
            throw new NullReferenceException("Unable to get token from body JSON");
        var xsrfToken = htmxHeaderConfig["X-XSRF-TOKEN"].ToString();
        return xsrfToken ?? throw new NullReferenceException("Unable to extract token from htmx headers config");
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

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Timo") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}