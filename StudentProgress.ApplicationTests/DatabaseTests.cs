using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Data.Sqlite;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Tool.hbm2ddl;
using StudentProgress.Application;
using StudentProgress.Application.Groups;
using System;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StudentProgress.Web.Utils;

namespace StudentProgress.ApplicationTests
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly string _database = "student-progress-new";

        private readonly string _connectionString =
            $"User ID=timodb;Password=DUKfxJCySEPS4;Host=localhost;Port=5432;Database=postgres;";

        private readonly SqliteConnection _connection;
        protected ISessionFactory SessionFactory { get; private set; }

        public DatabaseTests()
        {
            var connectionString = _connectionString.Replace("postgres", _database);
            Database.EnsureDatabase(_connectionString, _database);
            var serviceProvider = new ServiceCollection()
                .ConfigureMigrations(connectionString)
                .BuildServiceProvider(false);

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            SessionFactory = Fluently.Configure()
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Group>())
                .Database(PostgreSQLConfiguration.Standard.ConnectionString(connectionString))
                .BuildSessionFactory();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(@"
ALTER SEQUENCE hibernate_sequence RESTART WITH 1;
DELETE FROM ""StudentStudentGroup"";
DELETE FROM ""ProgressUpdate"";
DELETE FROM ""Student"";
DELETE FROM ""Group"";");
            }
        }

        public void Dispose()
        {
        }
    }
}