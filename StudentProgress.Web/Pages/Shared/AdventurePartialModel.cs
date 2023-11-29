namespace StudentProgress.Web.Pages.Shared
{
    public record AdventurePartialModel<T>(T Data, int AdventureId, int? QuestId, int? PersonId);
}
