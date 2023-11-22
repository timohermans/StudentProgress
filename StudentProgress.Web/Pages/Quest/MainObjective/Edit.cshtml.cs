using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Data;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.Quest.MainObjective
{
    public class EditModel(WebContext db) : PageModel
    {
        public int Id { get; set; }

        [BindProperty]
        public required string MainObjective { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Id = id;
            var quest = await db.Quests.FindAsync(id);

            if (quest == null)
            {
                return NotFound();
            }

            MainObjective = quest.MainObjective ?? "";

            return Page();
        }

        public async Task<IActionResult> OnPatch(int id)
        {
             Id = id;
             var questLine = await db.Quests.FindAsync(id);
 
             if (questLine == null)
             {
                 return NotFound();
             }

             if (!ModelState.IsValid)
             {
                 return Page();
             }

             questLine.MainObjective = MainObjective;
             await db.SaveChangesAsync();
 
             return this.SeeOther("./Index", new { Id = id });           
        }
    }
}