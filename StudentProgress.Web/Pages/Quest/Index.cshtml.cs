using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Data;

namespace StudentProgress.Web.Pages.Quest;

public class Index(WebContext db, ILogger<Index> logger) : PageModel
{
    public required Core.Models.Quest Quest { get; set; }

    public async Task<IActionResult> OnGet(int id, int? personId)
    {
        logger.LogDebug("Loading quest with id {id} and person {personId}", id, personId);
        if (id == 0) return new EmptyResult();

        var quest = await db.Quests
            .Include(ql => ql.Objectives)
            .ThenInclude(o => o.Progresses.Where(p => personId == null ? p.Person.Id == 0 : p.Person.Id == personId))
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quest == null) return NotFound();

        Quest = quest;

        return Page();
    }

    public IActionResult OnGetCreateLink(int id)
    {
        return Partial("_CreateLink", id);
    }
}