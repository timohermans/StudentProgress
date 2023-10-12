using System.Collections.Generic;

namespace StudentProgress.Web.Lib.CanvasApi.Models;

public class Connection<T>
{
    public required List<T> Nodes { get; set; }
}
