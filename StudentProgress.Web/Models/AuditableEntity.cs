using System;
using System.ComponentModel.DataAnnotations;

namespace StudentProgress.Web.Models
{
    public abstract class AuditableEntity
    {
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }
    }
}
