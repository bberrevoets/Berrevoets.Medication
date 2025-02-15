using System.Net.Http.Headers;

using Berrevoets.Medication.RazorWebApp.Models;
using Berrevoets.Medication.ServiceDefaults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Berrevoets.Medication.RazorWebApp.Pages;

[Authorize]
public class EditProfileModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditProfileModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public ProfileInput ProfileData { get; set; } = new();

    private string? ApiToken => User.FindFirst("Token")?.Value;

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if the token exists and is valid.
        if (string.IsNullOrEmpty(ApiToken) || TokenHelper.IsTokenExpired(ApiToken))
        {
            // Sign out the user if token expired.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirect to login page, optionally passing the returnUrl.
            return RedirectToPage("Login", new { returnUrl = "/EditProfile" });
        }

        var client = _httpClientFactory.CreateClient("UserApi");
        if (!string.IsNullOrEmpty(ApiToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);

        var profile = await client.GetFromJsonAsync<ProfileResponse>("api/users/profile");
        if (profile == null)
        {
            TempData["NotificationTitle"] = "Error";
            TempData["NotificationMessage"] = "Unable to load profile.";
            TempData["NotificationType"] = "error";
            return RedirectToPage("Index");
        }

        ProfileData.Username = profile.Username;
        ProfileData.Email = profile.Email;
        ProfileData.PhoneNumber = profile.PhoneNumber;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("UserApi");
        if (!string.IsNullOrEmpty(ApiToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);

        // Build the update request
        var updateRequest = new ProfileInput
        {
            Username = ProfileData.Username, // new username if changed
            Email = ProfileData.Email,
            PhoneNumber = ProfileData.PhoneNumber
        };

        var response = await client.PutAsJsonAsync("api/users/profile", updateRequest);
        if (response.IsSuccessStatusCode)
        {
            TempData["NotificationTitle"] = "Success";
            TempData["NotificationMessage"] = "Profile updated successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage("Catalog");
        }

        TempData["NotificationTitle"] = "Error";
        TempData["NotificationMessage"] = "Profile update failed.";
        TempData["NotificationType"] = "error";
        return Page();
    }
}