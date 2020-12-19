using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentProgress.Core.Entities;
using StudentProgress.CoreTests.UseCases;
using Xunit;

namespace StudentProgress.CoreTests
{
    public class DatabaseFixture
    {
        private readonly string _database = "student-progress-new";
        private readonly string _connectionString =
            $"User ID=timodb;Password=DUKfxJCySEPS4;Host=localhost;Port=5432;Database=student-progress-test;";

        public string ConnectionString { get; private set; }

        public DataMother DataMother { get; }

        public DbContextOptions<ProgressContext> ContextOptions { get; }
        
        public DatabaseFixture()
        {
            SetConnectionString();
            
            ContextOptions = new DbContextOptionsBuilder<ProgressContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            DataMother = new DataMother(ContextOptions);

            using var context = new ProgressContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        private void SetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__Test");
            var cString = configuration.GetConnectionString("Default");
            ConnectionString = envCString ?? cString;
            
            Console.WriteLine("THIS IS THE CONNECTIONSTRING");
            Console.WriteLine(ConnectionString);
            Console.WriteLine(cString);
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