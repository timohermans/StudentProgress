using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace StudentProgress.Web.Data
{
    public class ProgressContext : DbContext
    {
        public ProgressContext(DbContextOptions<ProgressContext> options)
            : base(options)
        {
        }

        public DbSet<StudentProgress.Web.Models.StudentGroup> StudentGroup { get; set; }

        public DbSet<StudentProgress.Web.Models.Student> Student { get; set; }
        public DbSet<StudentProgress.Web.Models.ProgressUpdate> ProgressUpdate { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.UpdateAuditableEntities();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
