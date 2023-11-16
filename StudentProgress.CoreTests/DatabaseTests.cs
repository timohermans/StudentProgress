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
            using var dataContext = Fixture.CreateWebContext();
            dataContext.Database.ExecuteSqlRaw(@"
DELETE FROM People;
DELETE FROM Adventures;
DELETE FROM __EFMigrationsHistory;
UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'People';
UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Adventures';
");
        }
    }
}