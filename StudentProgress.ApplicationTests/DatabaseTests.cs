using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Data.Sqlite;
using NHibernate;
using StudentProgress.Application.Groups;
using System;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StudentProgress.Application;
using StudentProgress.Web.Utils;
using Xunit;

namespace StudentProgress.ApplicationTests
{
    public abstract class DatabaseTests : IDisposable
    {
        private readonly DatabaseFixture _fixture;

        public DatabaseTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            CleanupData(fixture);
        }

        private static void CleanupData(DatabaseFixture fixture)
        {
            using var connection = new NpgsqlConnection(fixture.ConnectionString);
            connection.Execute(@"
ALTER SEQUENCE hibernate_sequence RESTART WITH 1;
DELETE FROM ""StudentStudentGroup"";
DELETE FROM ""ProgressUpdate"";
DELETE FROM ""Student"";
DELETE FROM ""Group"";");
        }

        public UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(_fixture.SessionFactory);
        }

        public void Dispose()
        {
            CleanupData(_fixture);
        }
    }
}