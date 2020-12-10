using System;
using System.ComponentModel.DataAnnotations;

namespace StudentProgress.Core.Entities
{
    public abstract class AuditableEntity
    {
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }
    }
}
