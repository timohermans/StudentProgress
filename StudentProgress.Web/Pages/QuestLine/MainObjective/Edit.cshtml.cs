using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Data;
using System.Threading;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.QuestLine.MainObjective
{
    public class EditModel(WebContext db) : PageModel
    {
        public int Id { get; set; }

        [BindProperty]
        public required string MainObjective { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Id = id;
            var questLine = await db.QuestLines.FindAsync(id);

            if (questLine == null)
            {
                return NotFound();
            }

            MainObjective = questLine.MainObjective ?? "";

            return Page();
        }

        public async Task<IActionResult> OnPatch(int id)
        {
             Id = id;
             var questLine = await db.QuestLines.FindAsync(id);
 
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