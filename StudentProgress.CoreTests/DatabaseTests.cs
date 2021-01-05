using System;
using Dapper;
using Npgsql;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests
{
    public class DatabaseTests
    {
        protected readonly DatabaseFixture Fixture;

        protected DatabaseTests(DatabaseFixture fixture)
        {
            Fixture = fixture;
            CleanupData();
        }

        private void CleanupData()
        {
            using var connection = new NpgsqlConnection(Fixture.ConnectionString);
            connection.Execute(@"
DELETE FROM ""StudentStudentGroup"";
DELETE FROM ""ProgressUpdate"";
DELETE FROM ""Student"";
DELETE FROM ""StudentGroup"";");
        }
    }
}