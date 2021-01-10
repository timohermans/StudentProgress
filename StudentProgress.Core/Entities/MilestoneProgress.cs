using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgress.Core.Entities
{
  public class MilestoneProgress : AuditableEntity<int>
  {
    public Milestone Milestone { get; set; }
    public Rating Rating { get; set; }
    public string? Comment { get; set; }

#nullable disable
    private MilestoneProgress() { }
#nullable enable

    public MilestoneProgress(Rating rating, Milestone milestone, string? comment)
    {
      Rating = rating;
      Milestone = milestone;
      Comment = comment;
    }

    public void Update(string? comment, Rating rating)
    {
      Comment = comment;
      Rating = rating;
    }
  }
}
