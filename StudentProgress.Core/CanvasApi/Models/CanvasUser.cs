﻿using System.Text.Json.Serialization;

namespace StudentProgress.Core.CanvasApi.Models;

public class CanvasUser
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
}
