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

namespace StudentProgress.ApplicationTests
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly string _connectionSring = $"DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared";
        private readonly SqliteConnection _connection;
        protected ISessionFactory SessionFactory { get; private set; }

        public DatabaseTests()
        {
            var configuration = Fluently.Configure()
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Group>())
                           .Database(SQLiteConfiguration.Standard.ConnectionString(_connectionSring))
                           .ExposeConfiguration(BuildSchema)
                           .BuildConfiguration();
            _connection = new SqliteConnection(_connectionSring);
            _connection.Open();

            SessionFactory = configuration.BuildSessionFactory();
        }

        private void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, true);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
