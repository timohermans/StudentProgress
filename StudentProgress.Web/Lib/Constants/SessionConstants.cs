namespace StudentProgress.Web.Lib.Constants;

public static class SessionKeys
{
    /// <summary>
    /// Key used for the person ID in the Adventure/Index page and HTMX partials.
    /// The reason this is created because at the time of typing personId had to be
    /// passed around among the many small HTMX requests. To solve this, this personId is introduced.
    /// </summary>
    public const string PersonId = "PersonId";
    /// <summary>
    /// Key used for the quest ID in the Adventure/Index page and HTMX partials.
    /// I want to try make the entire adventure details as interactive as possible
    /// </summary>
    public const string QuestId = "QuestId";
}