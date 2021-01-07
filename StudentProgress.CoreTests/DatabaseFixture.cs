using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Npgsql;
using StudentProgress.Core.Entities;
using StudentProgress.CoreTests.UseCases;
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
                .UseLoggerFactory(new LoggerFactory(new[] {new DebugLoggerProvider()}))
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

            if (string.IsNullOrWhiteSpace(envCString))
            {
                throw new Exception("Nope");
            }
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