namespace StudentProgress.Core.Extensions
{
    public static class PrimitiveExtensions
    {
        public static bool IsInRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool IsBetween(this DateTime date, DateTime min, DateTime max)
        {
            return date > min && date < max;
        }
    }
}