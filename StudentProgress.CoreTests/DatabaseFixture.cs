using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentProgress.Core.Entities;
using StudentProgress.Web.Lib.Data;
using Xunit;

namespace StudentProgress.CoreTests
{
    public class DatabaseFixture
    {
        public string ConnectionString { get; private set; }

        public DataMother DataMother { get; }
        public WebDataMother WebDataMother { get; }

        public DbContextOptions<ProgressContext> ContextOptions { get; }
        public DbContextOptions<WebContext> WebContextOptions { get; private set; }

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

            WebContextOptions = CreateWebContextOptions();
            WebDataMother = new WebDataMother(WebContextOptions);
            using var webContext = new WebContext(WebContextOptions);
            webContext.Database.EnsureDeleted();
            webContext.Database.Migrate();
        }

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__Test");
            var cString = configuration.GetConnectionString("Default");

            return ConnectionString = envCString ?? cString ??
                throw new NullReferenceException(
                    "Connectionstring could not be found in either env var or appsettings");
        }

        public ProgressContext CreateDbContext()
        {
            return new ProgressContext(ContextOptions);
        }

        private DbContextOptions<WebContext> CreateWebContextOptions()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__WebContext");
            var cString = configuration.GetConnectionString("WebContext");

            var connectionString = envCString ?? cString ??
                throw new NullReferenceException(
                    "ConnectionString__WebContext could not be found in either env var or appsettings");

            return new DbContextOptionsBuilder<WebContext>()
                .UseSqlite(connectionString)
                .Options;
        }

        public WebContext CreateWebContext()
        {
            return new WebContext(WebContextOptions);
        }
    }
}