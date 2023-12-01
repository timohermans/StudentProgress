using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Pages.Shared;

namespace StudentProgress.Web.Pages.QuestLines;

public class IndexModel(WebContext db) : PageModel
{
    public int AdventureId { get; set; }
    public int? QuestId { get; set; }
    public int? PersonId { get; set; }
    public List<Core.Models.QuestLine> QuestLines { get; set; }

    public async Task<IActionResult> OnGet(int adventureId, int? questId, int? personId)
    {
        AdventureId = adventureId;
        QuestId = questId;
        PersonId = personId;
        
        var adventure = await db.Adventures
            .Include(a => a.QuestLines)
            .ThenInclude(ql => ql.Quests)
            .ThenInclude(q => q.Objectives)
            .ThenInclude(o => o.Progresses)
            .Where(a => a.Id == adventureId)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (adventure == null)
        {
            return this.NotFoundToBody();
        }

        QuestLines = adventure.QuestLines.ToList();
        
        return Page();
    }
}
