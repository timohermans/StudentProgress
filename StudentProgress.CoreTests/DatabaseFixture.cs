using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.CoreTests.UseCases;
using Xunit;

namespace StudentProgress.CoreTests
{
    public class DatabaseFixture
    {
        private readonly string _database = "student-progress-new";
        private readonly string _connectionString =
            $"User ID=timodb;Password=DUKfxJCySEPS4;Host=localhost;Port=5432;Database=postgres;";

        public string ConnectionString => _connectionString.Replace("postgres", _database);

        public DataMother DataMother { get; }

        public DbContextOptions<ProgressContext> ContextOptions { get; }
        
        public DatabaseFixture()
        {
            // Database.EnsureDatabase(_connectionString, _database);
            ContextOptions = new DbContextOptionsBuilder<ProgressContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            DataMother = new DataMother(ContextOptions);

            using var context = new ProgressContext(ContextOptions);
            context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();
            context.Database.Migrate();
        }

        public ProgressContext CreateDbContext()
        {
            return new ProgressContext(ContextOptions);
        }
    }
    
    [CollectionDefinition("db")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}