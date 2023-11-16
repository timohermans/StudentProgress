using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Data;
using System.Linq;

namespace StudentProgress.Web.Pages.QuestLine;

public class Index(ILogger<Index> logger, WebContext db) : PageModel
{
    public int Id { get; set; }
    public required Models.QuestLine QuestLine { get; set; }

    public async Task<IActionResult> OnGet(int id)
    {
        logger.LogDebug("Questline fetch for {id}", id);
        if (id == 0) return new EmptyResult();
        Id = id;

        var questLine = await db.QuestLines
            .Include(ql => ql.Quests)
            .OrderBy(ql => ql.Order)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questLine == null) return NotFound();

        QuestLine = questLine;

        return Page();
    }
}