using System;
using FluentMigrator.Runner;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Cfg;
using StudentProgress.Application.Groups;
using StudentProgress.Application.Migrations;
using StudentProgress.Web.Utils;
using Xunit;

namespace StudentProgress.ApplicationTests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly string _database = "student-progress-new";
        private readonly string _connectionString =
            $"User ID=timodb;Password=DUKfxJCySEPS4;Host=localhost;Port=5432;Database=postgres;";

        public ISessionFactory SessionFactory { get; private set; }
        public string ConnectionString { get; private set; }
        
        public DatabaseFixture()
        {
            ConnectionString = _connectionString.Replace("postgres", _database);
            Database.EnsureDatabase(_connectionString, _database);
            var serviceProvider = new ServiceCollection()
                .ConfigureMigrations(ConnectionString)
                .BuildServiceProvider(false);

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            var rawConfig = new Configuration();
            rawConfig.SetNamingStrategy(new PostgreSqlNamingStrategy());
            SessionFactory = Fluently.Configure(rawConfig)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Group>())
                .Database(PostgreSQLConfiguration.Standard.ConnectionString(ConnectionString))
                .BuildSessionFactory();            
        }
        
        public void Dispose()
        {
            SessionFactory.Dispose();
        }
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}