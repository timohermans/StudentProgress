using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Data;
using System.Linq;

namespace StudentProgress.Web.Pages.QuestLines;

public class IndexModel(WebContext db) : PageModel
{
    public async Task OnGet(int adventureId, int? questId, int? personId)
    {
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
    }
}
