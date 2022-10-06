using Microsoft.EntityFrameworkCore;

namespace StudentProgress.CoreTests
{
    // This class is created to provide:
    // 1: a property that exposes the fixture (instead of injecting it and assigning it to a field)
    // 2: cleans up the database after every test. CollectionFixtures don't do this
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
DELETE FROM MilestoneProgress;
DELETE FROM Milestone;
DELETE FROM StudentStudentGroup;
DELETE FROM ProgressUpdate;
DELETE FROM Student;
DELETE FROM StudentGroup;
DELETE FROM Settings;");
        }
    }
}