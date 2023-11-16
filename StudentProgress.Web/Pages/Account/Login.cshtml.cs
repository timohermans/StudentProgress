using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Infrastructure;
using StudentProgress.Core.Models;

namespace StudentProgress.Web.Pages.Account;

public class Login(IConfiguration config, ILogger<Login> logger) : PageModel
{
    private readonly static LoginThrottler _loginThrottler = new(new DateProvider());

    [BindProperty] public new required User User { get; set; }

    public void OnGet()
    {
        logger.LogInformation("Someone is trying to log in.");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_loginThrottler.GetSecondsLeftToTryAgain() > 0)
        {
            ModelState.AddModelError("throttled", "Too many login attempts. Wait some time to try again");
            return Page();
        }

        var adminUsername = config.GetValue<string>("Admin:Username");
        var adminPassword = config.GetValue<string>("Admin:Password");

        if (adminUsername != User.Email || adminPassword != User.Password)
        {
            _loginThrottler.Throttle();
            logger.LogWarning("Someone failed login with username {Email} and a given password. Throttler: {SecondsLeft}s", User.Email, _loginThrottler.GetSecondsLeftToTryAgain());
            ModelState.AddModelError("login", "Username or password is incorrect");
            return Page();
        }

        logger.LogInformation("Successful login!");
        var claims = new List<Claim>
        {
            new("user", adminUsername),
            new("role", "Member")
        };
        await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
        _loginThrottler.Reset();

        if (Url.IsLocalUrl(returnUrl))
        {
            return RedirectToPage("TwoFactorLogin", null, routeValues: new { returnUrl });
        }

        return RedirectToPage("TwoFactorLogin");
    }
}