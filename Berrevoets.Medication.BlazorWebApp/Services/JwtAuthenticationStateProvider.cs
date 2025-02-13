using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Berrevoets.Medication.BlazorWebApp.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "jwtToken";
    private readonly ProtectedSessionStorage _sessionStorage;

    private string? _jwtToken;

    public JwtAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        _jwtToken = token;
        await _sessionStorage.SetAsync(TokenKey, token);
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        _jwtToken = null;
        await _sessionStorage.DeleteAsync(TokenKey);
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Attempt to retrieve token from session storage if not already loaded
        if (string.IsNullOrWhiteSpace(_jwtToken))
        {
            var storedResult = await _sessionStorage.GetAsync<string>(TokenKey);
            if (storedResult.Success)
            {
                _jwtToken = storedResult.Value;
            }
        }

        if (!string.IsNullOrWhiteSpace(_jwtToken))
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(_jwtToken), "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}