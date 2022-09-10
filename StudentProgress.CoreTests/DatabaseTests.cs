using Microsoft.EntityFrameworkCore;

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
            using var dbContext = Fixture.CreateDbContext();
            dbContext.Database.ExecuteSqlRaw(@"
DELETE FROM ""MilestoneProgress"";
DELETE FROM ""Milestone"";
DELETE FROM ""StudentStudentGroup"";
DELETE FROM ""ProgressUpdate"";
DELETE FROM ""Student"";
DELETE FROM ""StudentGroup"";
DELETE FROM Settings;");
        }
    }
}