using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.Account;

public class Login : PageModel
{
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

        var adminUsername = _config.GetValue<string>("Admin:Username");
        var adminPassword = _config.GetValue<string>("Admin:Password");

        if (adminUsername != User.Email || adminPassword != User.Password)
        {
            _logger.LogWarning($"Someone failed login with username {User.Email} and a given password");
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

        if (Url.IsLocalUrl(returnUrl))
        {
            return RedirectToPage("TwoFactorLogin", null, routeValues: new { returnUrl });
        }
        else
        {
            return RedirectToPage("TwoFactorLogin");
        }

        return Page();
    }
}