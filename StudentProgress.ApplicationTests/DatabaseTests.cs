using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using StudentProgress.Application.Groups;
using System;
using System.IO;

namespace StudentProgress.ApplicationTests
{
    public abstract class DatabaseTests : IDisposable
    {
        public ISessionFactory SessionFactory { get; private set; }
        public ISession Session { get; private set; }

        public DatabaseTests()
        {
            SessionFactory = Fluently.Configure()
                           .Database(SQLiteConfiguration.Standard
                               .UsingFile("db.sqlite"))
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Group>())
                           .ExposeConfiguration(BuildSchema)
                            .BuildSessionFactory();

            Session = SessionFactory.OpenSession();
        }

        private void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists("db.sqlite"))
                File.Delete("db.sqlite");

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, true);

        }

        public void Dispose()
        {
            Session.Dispose();
        }
    }
}
