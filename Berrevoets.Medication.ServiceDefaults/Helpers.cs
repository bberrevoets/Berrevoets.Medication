using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Berrevoets.Medication.ServiceDefaults;

public static class JwtHelper
{
    public static Guid? GetUserId(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        // Adjust the claim type as needed: "id" or "sub"
        var userId = token.Claims.FirstOrDefault(c => c.Type is "id")?.Value;

        if (Guid.TryParse(userId, out var id)) return id;

        return null;
    }
}