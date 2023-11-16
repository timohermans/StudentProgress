using System.Security.Claims;
using StudentProgress.Core.Constants;

namespace StudentProgress.Core.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static bool IsFullyAuthenticated(this ClaimsPrincipal user)
    {
        return (user.Identity?.IsAuthenticated ?? false) && user.IsTwoFactorAuthenticated();
    }

    public static bool IsTwoFactorAuthenticated(this ClaimsPrincipal user)
    {
        return user.HasClaim(AuthConstants.TwoFactorLoginPolicy, AuthConstants.TwoFactorLoginClaimValue);
    }
}