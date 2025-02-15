using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Berrevoets.Medication.ServiceDefaults;

public static class TokenHelper
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

    public static bool IsTokenExpired(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            return true;
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);

        // The exp claim is in Unix time (seconds)
        var expClaim = token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        
        // if no exp claim assume it's expired.
        if (expClaim == null || !long.TryParse(expClaim, out long expSeconds)) return true;

        var expDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
        return expDateTimeUtc < DateTime.UtcNow;
    }
}