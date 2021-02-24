using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
  public class ProgressTag: Entity<int>
  {
    public Name Name { get; }
    private readonly List<ProgressUpdate> _updates = new();
    public IReadOnlyList<ProgressUpdate> Updates => _updates;

    public ProgressTag(Name name)
    {
      Name = name;
    }
  }
}