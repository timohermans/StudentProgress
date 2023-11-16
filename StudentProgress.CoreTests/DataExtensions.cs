namespace StudentProgress.CoreTests
{
    public static class DataExtensions
    {
        public static T ShouldExist<T>(this T entity)
        {
            entity.Should().NotBe(null);
            return entity;
        }
    }
}