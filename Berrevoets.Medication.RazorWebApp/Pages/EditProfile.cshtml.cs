using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
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
        if (!User.Identity.IsAuthenticated) return RedirectToPage("Login");

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

public class ProfileInput
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Display(Name = "Phone Number")] public string? PhoneNumber { get; set; }
}

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public string Role { get; set; } = "";
}