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

#nullable disable
        private MilestoneProgress() { }
#nullable enable
    }
}
