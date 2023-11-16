using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Pages.Account;

public class AccessDenied : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (!HttpContext.User.IsTwoFactorAuthenticated())
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Account/Login");
        }
        
        return Page();
    }
}