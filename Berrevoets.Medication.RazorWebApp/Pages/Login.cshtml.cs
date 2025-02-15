using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Berrevoets.Medication.RazorWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginData LoginData { get; set; } = new LoginData();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("UserApi");
            var response = await client.PostAsJsonAsync("api/users/login", LoginData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    // Create claims for the user, including storing the token in a claim.
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, LoginData.Username),
                        new Claim("Token", result.Token),
                        new Claim("id", result.Id)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user using cookie authentication.
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    TempData["NotificationTitle"] = "Success";
                    TempData["NotificationMessage"] = "Logged in successfully.";
                    TempData["NotificationType"] = "success";

                    return RedirectToPage("Catalog");
                }
            }

            TempData["NotificationTitle"] = "Error";
            TempData["NotificationMessage"] = "Login failed. Check your credentials.";
            TempData["NotificationType"] = "error";
            return Page();
        }
    }

    public class LoginData
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResult
    {
        public string Token { get; set; } = "";
        public string Id { get; set; } = "";
    }
}
