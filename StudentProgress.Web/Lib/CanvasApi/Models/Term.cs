using System;
using System.Text.Json.Serialization;

namespace StudentProgress.Web.Lib.CanvasApi.Models;

public class Term
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
}
