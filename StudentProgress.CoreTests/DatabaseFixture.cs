using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentProgress.Core.Entities;
using Xunit;

namespace StudentProgress.CoreTests
{
    public class DatabaseFixture
    {
        public string ConnectionString { get; private set; }

        public DataMother DataMother { get; }

        public DbContextOptions<ProgressContext> ContextOptions { get; }

        public DatabaseFixture()
        {
            ConnectionString = GetConnectionString();

            ContextOptions = new DbContextOptionsBuilder<ProgressContext>()
                .UseSqlite(ConnectionString)
                .Options;

            DataMother = new DataMother(ContextOptions);

            using var context = new ProgressContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__Test");
            var cString = configuration.GetConnectionString("Default");

            return ConnectionString = envCString ?? cString ?? throw new NullReferenceException("Connectionstring could not be found in either env var or appsettings");
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