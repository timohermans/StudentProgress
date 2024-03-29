﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;

namespace StudentProgress.Core.CanvasApi;

public class CanvasClient : ICanvasClient
{
    private readonly HttpClient _client;
    private readonly string? _url;

    public CanvasClient(HttpClient client, ICanvasApiConfig config)
    {
        var apiKey = config.CanvasApiKey;
        _client = client;
        _url = config.CanvasApiUrl;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<CanvasResponse<T>?> GetAsync<T>(string query, CancellationToken token)
    {
        var body = new { operationName = "MyQuery", query, variables = (string)null! };
        var response = await _client.PostAsJsonAsync(_url, body, token);
        var json = await response.Content.ReadAsStringAsync(token);
        return JsonSerializer.Deserialize<CanvasResponse<T>>(json,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
    }
}
