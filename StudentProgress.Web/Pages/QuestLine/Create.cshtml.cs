using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Pages.QuestLine;

public class CreateModel : PageModel
{
    public int AdventureId { get; set; }
    public int? PersonId { get; set; }
    [BindProperty] public required Models.QuestLine QuestLine { get; set; }

    public void OnGet(int adventureId, int? personId)
    {
        AdventureId = adventureId;
        PersonId = personId;
    }

    public IActionResult OnPost(int adventureId, int? personId)
    {
        OnGet(adventureId, personId);
        
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // set Order

        return Partial("_Row", QuestLine);
    }
}