namespace StudentProgress.Core.CanvasApi.Models;

public class Connection<T>
{
    public required List<T> Nodes { get; set; }
}
