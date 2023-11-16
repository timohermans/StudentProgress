using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Data;

namespace StudentProgress.Web.Pages.QuestLine.MainObjective
{
    public class IndexModel(WebContext db) : PageModel
    {
        public Core.Models.QuestLine? QuestLine { get; set; }
        
        public async Task OnGet(int id)
        {
            QuestLine = await db.QuestLines
                .FindAsync(id);
        }
    }
}
