using System;
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
        public DbSet<Milestone> Milestones => Set<Milestone>();
        public DbSet<MilestoneProgress> MilestoneProgresses => Set<MilestoneProgress>();

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
                g.Property(p => p.Period)
                    .HasConversion(p => p.StartDate, y => (Period) y)
                    .HasDefaultValue((Period)DateTime.MinValue);
                g.HasMany(p => p.Students).WithMany(s => s.StudentGroups);
                g.HasMany(p => p.Milestones).WithOne(m => m.StudentGroup);
                g.HasIndex(p => p.Name).IsUnique();
            });

            modelBuilder.Entity<ProgressUpdate>(e =>
            {
                e.ToTable("ProgressUpdate");
                e.HasKey(p => p.Id);
                e.HasOne(p => p.Student).WithMany(s => s.ProgressUpdates);
                e.HasOne(p => p.Group).WithMany();
                e.HasMany(p => p.MilestonesProgress).WithOne(p => p.ProgressUpdate);
            });

            modelBuilder.Entity<Student>(e =>
            {
                e.ToTable("Student");
                e.HasKey(p => p.Id);
                e.HasIndex(p => p.Name).IsUnique();
            });

            modelBuilder.Entity<Milestone>(e =>
            {
                e.ToTable("Milestone");
                e.HasKey(p => p.Id);
                e.Property(p => p.LearningOutcome).HasConversion(p => p.Value, p => Name.Create(p).Value);
                e.Property(p => p.Artefact).HasConversion(p => p.Value, p => Name.Create(p).Value);
                e.HasIndex(p => new { p.StudentGroupId, p.Artefact, p.LearningOutcome }).IsUnique();
            });

            modelBuilder.Entity<MilestoneProgress>(e =>
            {
                e.ToTable("MilestoneProgress");
                e.HasKey(p => p.Id);
                e.HasOne(p => p.Milestone)
                    .WithMany();
            });
        }
    }
}