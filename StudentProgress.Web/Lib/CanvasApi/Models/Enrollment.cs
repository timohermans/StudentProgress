using System.Text.Json.Serialization;

namespace StudentProgress.Web.Lib.CanvasApi.Models;

public class Enrollment
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public CanvasUser? User { get; set; }
    public Section? Section { get; set; }
}
