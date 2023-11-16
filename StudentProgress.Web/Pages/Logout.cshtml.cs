using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Pages;

public class LogoutModel : PageModel
{
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/Account/Login");
    }
}
