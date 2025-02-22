using System.Security.Claims;

namespace TrelloApi.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user?.FindFirst("UserId");
        return claim != null && int.TryParse(claim.Value, out int userId) ? userId : null;
    }
}