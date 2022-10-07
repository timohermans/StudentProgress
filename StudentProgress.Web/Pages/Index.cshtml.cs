using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/StudentGroups/Index");
    }
}
