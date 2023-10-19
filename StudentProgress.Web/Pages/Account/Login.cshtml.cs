using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Infrastructure;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.Account;

public class Login : PageModel
{
    private static LoginThrottler _loginThrottler = new(new DateProvider());

    private readonly IConfiguration _config;
    private ILogger<Login> _logger;

    [BindProperty] public new required User User { get; set; }

    public Login(IConfiguration config, ILogger<Login> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Someone is trying to log in.");
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

        var adminUsername = _config.GetValue<string>("Admin:Username");
        var adminPassword = _config.GetValue<string>("Admin:Password");

        if (adminUsername != User.Email || adminPassword != User.Password)
        {
            _loginThrottler.Throttle();
            _logger.LogWarning($"Someone failed login with username {User.Email} and a given password. Throttler: {_loginThrottler.GetSecondsLeftToTryAgain()}s");
            ModelState.AddModelError("login", "Username or password is incorrect");
            return Page();
        }

        _logger.LogInformation("Successful login!");
        var claims = new List<Claim>
        {
            new Claim("user", adminUsername),
            new Claim("role", "Member")
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