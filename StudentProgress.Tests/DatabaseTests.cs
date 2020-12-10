using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StudentProgress.Web.Data;

namespace StudentProgress.Tests
{
    public class DatabaseTests: IDisposable
    {
        private readonly DbConnection _connection;
        protected DbContextOptions<ProgressContext> ContextOptions { get; }

        protected DatabaseTests()
        {
            ContextOptions = new DbContextOptionsBuilder<ProgressContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
            Seed();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
        
        private void Seed()
        {
            using var dbContext = new ProgressContext(ContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        public void Dispose() => _connection.Dispose();
    }
}