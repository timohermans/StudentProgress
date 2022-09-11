using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public abstract class AuditableEntity<T> : Entity<T>
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
