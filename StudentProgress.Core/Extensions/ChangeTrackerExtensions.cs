using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void UpdateAuditableEntities(this ChangeTracker changeTracker)
        {
            var auditableEntities = changeTracker.Entries<AuditableEntity<int>>();
            foreach (var entity in auditableEntities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedDate = DateTime.UtcNow;
                }

                if (entity.State == EntityState.Added || entity.State == EntityState.Modified)
                {
                    entity.Entity.UpdatedDate = DateTime.UtcNow;
                }
            }
        }
    }
}
