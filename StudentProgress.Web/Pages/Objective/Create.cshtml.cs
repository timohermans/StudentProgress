using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Data;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.Objective;

public class Create(WebContext db) : PageModel
{
    public int AdventureId { get; set; }
    public int? PersonId { get; set; }

    [BindProperty] public required int QuestId { get; set; }
    [BindProperty] public required string Name { get; set; }

    public async Task<IActionResult> OnGet(int questId, int? personId)
    {
        PersonId = personId;
        QuestId = questId;

        Core.Models.Quest? quest = await db.Quests.FindAsync(QuestId);

        if (quest == null)
        {
            return this.NotFoundToBody();
        }

        AdventureId = await db.Adventures
            .Where(a => a.QuestLines.Any(ql => ql.Quests.Any(q => q.Id == questId)))
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        return Page();

    }

    public async Task<IActionResult> OnPost(int? personId)
    {
        var quest = await db.Quests
            .Include(q => q.Objectives)
            .FirstOrDefaultAsync(q => q.Id == QuestId);

        if (quest == null)
        {
            return this.NotFoundToBody();
        }
        
        if (!ModelState.IsValid)
        {
            return this.PageTo("#objectiveForm", "outerHTML");
        }

        Core.Models.Objective objective = new Core.Models.Objective
        {
            Name = Name,
            Color = "#e6007e"
        };

        quest.Objectives.Add(objective);
        await db.SaveChangesAsync();

        return this.SeeOther("/Quest/Index", new { Id = QuestId, PersonId = personId});
    }
}