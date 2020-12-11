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

        public DbSet<StudentGroup> StudentGroup => Set<StudentGroup>();
        public DbSet<Student> Student => Set<Student>();
        public DbSet<ProgressUpdate> ProgressUpdate => Set<ProgressUpdate>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.UpdateAuditableEntities();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
