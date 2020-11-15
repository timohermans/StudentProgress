using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Models
{
    public class ProgressUpdate : AuditableEntity
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public string Feedback { get; set; }
        public string Feedup { get; set; }
        public string Feedforward { get; set; }
        public Feeling ProgressFeeling { get; set; }
    }
}
