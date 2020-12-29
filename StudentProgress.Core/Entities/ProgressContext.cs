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

        public DbSet<StudentGroup> Groups => Set<StudentGroup>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<ProgressUpdate> ProgressUpdates => Set<ProgressUpdate>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.UpdateAuditableEntities();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StudentGroup>(g =>
            {
                g.ToTable("StudentGroup");
                g.HasKey(p => p.Id);
                g.Property(p => p.Name)
                    .HasConversion(p => p.Value, p => Name.Create(p).Value);
                g.HasMany(p => p.Students).WithMany(s => s.Groups);
                g.HasIndex(p => p.Name).IsUnique();
            });


            modelBuilder.Entity<ProgressUpdate>(e =>
            {
                e.ToTable("ProgressUpdate");
                e.HasKey(p => p.Id);
                e.HasOne(p => p.Student).WithMany(s => s.ProgressUpdates);
                e.HasOne(p => p.Group).WithMany();
            });
            
            
            modelBuilder.Entity<Student>(e =>
            {
                e.ToTable("Student");
                e.HasKey(p => p.Id);
            });
        }
    }
}