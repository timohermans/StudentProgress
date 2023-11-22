namespace StudentProgress.Web.Pages.Quest;

public record ListItemModel(int AdventureId, Core.Models.Quest Quest, int? PersonId = null, bool IsSelected = false);