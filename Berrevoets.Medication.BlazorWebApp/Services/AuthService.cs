using Microsoft.AspNetCore.Components.Authorization;

namespace Berrevoets.Medication.BlazorWebApp.Services;

public class AuthService : IAuthService
{
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;
    private readonly HttpClient _httpClient;

    public AuthService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("UserApi");
        _authenticationStateProvider = (JwtAuthenticationStateProvider)authenticationStateProvider;
    }

    public async Task<bool> Login(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/login", model);
        if (response.IsSuccessStatusCode)
        {
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (loginResult is not null && !string.IsNullOrEmpty(loginResult.Token))
            {
                await _authenticationStateProvider.MarkUserAsAuthenticated(loginResult.Token);
                return true;
            }
        }

        return false;
    }

    public async Task Logout()
    {
        await _authenticationStateProvider.MarkUserAsLoggedOut();
    }
}

public class LoginModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResult
{
    public string Token { get; set; } = "";
}