using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Core.Entities
{
    public class ProgressContext : DbContext
    {
        public ProgressContext(DbContextOptions<ProgressContext> options)
            : base(options)
        {
        }

        public DbSet<StudentGroup> StudentGroup { get; set; }

        public DbSet<Student> Student { get; set; }
        public DbSet<ProgressUpdate> ProgressUpdate { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.UpdateAuditableEntities();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
