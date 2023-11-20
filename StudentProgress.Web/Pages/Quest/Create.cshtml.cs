using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Data;

namespace StudentProgress.Web.Pages.Quest;

public class Create(WebContext db) : PageModel
{
    public int QuestLineId { get; set; }

    [BindProperty]
    public required string Name { get; set; }

    public async Task<IActionResult> OnGet(int questLineId)
    {
        QuestLineId = questLineId;

        var questLine = await db.QuestLines.FindAsync(questLineId);
        if (questLine == null)
        {
            return NotFound();
        }

        return Page();
    }
}