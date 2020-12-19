using Dapper;
using Npgsql;

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
DELETE FROM ""GroupStudent"";
DELETE FROM ""ProgressUpdate"";
DELETE FROM ""Student"";
DELETE FROM ""Group"";");
        }
    }
}