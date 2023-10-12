using System.Text.Json.Serialization;

namespace StudentProgress.Web.Lib.CanvasApi.Models;


public class Section
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string? Name { get; set; }
}
