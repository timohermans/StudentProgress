using System;
using StudentProgress.Application.Groups.UseCases;

namespace StudentProgress.Application.Common
{
    public abstract class AuditableEntity
    {
        public virtual DateTime CreatedAt { get; }
        public virtual DateTime ModifiedAt { get; private set; }

        protected AuditableEntity()
        {
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        protected void Update()
        {
            ModifiedAt = DateTime.UtcNow;
        }
    }
}