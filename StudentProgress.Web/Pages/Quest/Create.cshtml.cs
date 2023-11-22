using System.Linq;
using LanguageExt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Data;

namespace StudentProgress.Web.Pages.Quest;

public class Create(WebContext db, ILogger<Create> logger) : PageModel
{
    [BindProperty] public int QuestLineId { get; set; }
    [BindProperty] public int? PersonId { get; set; }

    [BindProperty] public required string Name { get; set; }

    public async Task<IActionResult> OnGet(int questLineId, int? personId)
    {
        QuestLineId = questLineId;
        PersonId = personId;

        var questLine = await db.QuestLines.FindAsync(questLineId);
        if (questLine == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        logger.LogDebug("Going to create a new quest");
        if (!ModelState.IsValid)
        {
            logger.LogInformation("Failed creating new quest, because model validation failed");
            return Page();
        }

        var questLine = await db.QuestLines.FindAsync(QuestLineId);
        if (questLine == null)
        {
            logger.LogWarning("Questline with id {QuestLineId} could not be found for new quest", QuestLineId);
            return NotFound();
        }

        var quest = new Core.Models.Quest
        {
            Name = Name
        };

        questLine.Quests.Add(quest);
        await db.SaveChangesAsync();
        logger.LogInformation("Quest {Name} ({Id}) successfully created", quest.Name, quest.Id);

        var adventureId =
            await db.Adventures
                .Where(a => a.QuestLines.Any(ql => ql.Quests.Any(q => q.Id == quest.Id)))
                .Select(a => a.Id)
                .FirstAsync();

        return Partial("_ListItem", new ListItemModel(adventureId, quest, PersonId));
    }
}