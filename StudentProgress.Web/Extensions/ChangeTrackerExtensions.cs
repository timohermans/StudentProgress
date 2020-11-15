using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudentProgress.Web.Models;
using System;

namespace StudentProgress.Web.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void UpdateAuditableEntities(this ChangeTracker changeTracker)
        {
            var auditableEntities = changeTracker.Entries<AuditableEntity>();
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
