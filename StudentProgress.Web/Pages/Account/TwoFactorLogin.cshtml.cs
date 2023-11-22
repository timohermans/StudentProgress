using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OtpNet;
using StudentProgress.Core.Constants;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Pages.Account;

public class TwoFactorLogin : PageModel
{
    private readonly ILogger<TwoFactorLogin> _logger;
    private readonly IConfiguration _config;

    [BindProperty] public required string Code { get; set; }

    public TwoFactorLogin(ILogger<TwoFactorLogin> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public IActionResult OnGet()
    {
        _logger.LogDebug("Someone is trying to 2 factor");
        _logger.LogDebug(string.Join(", ", HttpContext.User.Claims.Select(c => c.Value).ToList()));
        if (!(HttpContext.User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToPage("./Login");
        }

        if (HttpContext.User.IsTwoFactorAuthenticated())
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(string returnUrl = "/")
    {
        if (!(HttpContext.User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToPage("./Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var secret = _config.GetValue<string>("Admin:TwoFactorSecret");
        if (secret == null)
        {
            throw new NullReferenceException(
                "Two factor secret missing. Generate one with KeyGenerator.GenerateRandomKey(20) and Base32Encoding.ToString and put it in config");
        }

        var base32Bytes = Base32Encoding.ToBytes(secret);
        var otp = new Totp(base32Bytes);
        if (!otp.VerifyTotp(Code, out long timeStepMatched, new VerificationWindow()))
        {
            _logger.LogWarning("2FA failure");
            ModelState.AddModelError("incorrect", "One time password was incorrect");
            return Page();
        }

        _logger.LogInformation($"2FA success. Time step match: {timeStepMatched}");
        ((ClaimsIdentity)HttpContext.User.Identity).AddClaim(new Claim(AuthConstants.TwoFactorLoginPolicy,
            AuthConstants.TwoFactorLoginClaimValue));
        await HttpContext.SignInAsync(HttpContext.User);
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToPage("/Index");
    }
}