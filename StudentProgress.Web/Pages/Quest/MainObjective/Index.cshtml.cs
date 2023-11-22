using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Data;

namespace StudentProgress.Web.Pages.Quest.MainObjective
{
    public class IndexModel(WebContext db) : PageModel
    {
        public Core.Models.Quest? Quest { get; set; }
        
        public async Task OnGet(int id)
        {
            Quest = await db.Quests
                .FindAsync(id);
        }
    }
}
