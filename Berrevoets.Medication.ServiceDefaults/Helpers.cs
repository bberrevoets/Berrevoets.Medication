using System.IdentityModel.Tokens.Jwt;

namespace Berrevoets.Medication.ServiceDefaults;

public static class JwtHelper
{
    public static string? GetUserIdFromToken(string jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        // Adjust the claim type as needed: "id" or "sub"
        return token.Claims.FirstOrDefault(c => c.Type is "id" or JwtRegisteredClaimNames.Sub)?.Value;
    }
}