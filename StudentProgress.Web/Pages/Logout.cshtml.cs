using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("OpenIdConnect");
            return Redirect(User.FindFirst("iss").Value + $"/protocol/openid-connect/logout?redirect_uri={Request.Scheme}://{Request.Host.Value}");
        }
    }
}
