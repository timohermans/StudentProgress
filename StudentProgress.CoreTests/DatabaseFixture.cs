using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
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
        }

        public ProgressContext CreateDbContext()
        {
            return new ProgressContext(ContextOptions);
        }

        public IDbConnection CreateDbConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }

    [CollectionDefinition("db")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}