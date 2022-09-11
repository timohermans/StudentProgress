using System.Text.Json.Serialization;

namespace StudentProgress.Core.CanvasApi.Models;

public class Course
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Term? Term { get; set; }
    public Connection<Section>? SectionsConnection { get; set; }
    public Connection<Enrollment>? EnrollmentsConnection { get; set; }
}
