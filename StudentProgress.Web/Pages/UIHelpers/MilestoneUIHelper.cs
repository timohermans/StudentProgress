using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Pages.UIHelpers
{
    public static class MilestoneUiHelper
    {
        public static string RatingColor(Rating? rating, int? currentIndex = null)
        {
            if (!rating.HasValue || currentIndex.HasValue && currentIndex != (int) rating)
            {
                return "light";
            }

            return rating switch
            {
                Rating.Undefined => "dark",
                Rating.Orienting => "danger",
                Rating.Beginning => "warning",
                Rating.Proficient => "success",
                Rating.Advanced => "primary",
                _ => "light"
            };
        }
    }
}