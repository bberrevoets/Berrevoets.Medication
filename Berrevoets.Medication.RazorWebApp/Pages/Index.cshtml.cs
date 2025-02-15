using System.Net.Http.Headers;
using Berrevoets.Medication.RazorWebApp.Models;
using Berrevoets.Medication.ServiceDefaults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace Berrevoets.Medication.RazorWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // Property to indicate if the user is logged in
    public bool IsLoggedIn { get; set; }

    // Loaded user profile summary
    public UserProfileSummary? Profile { get; set; }

    // Medicine uses summary: list of medicines added by the user,
    // with calculated remaining days.
    public List<MedicineUseSummary> MedicineUses { get; set; } = [];

    // Extract token from the authenticated user’s claims.
    public string? ApiToken => User.FindFirst("Token")?.Value;

    // This method runs on every GET request.
    public async Task<IActionResult> OnGetAsync()
    {
        // Check authentication:
        IsLoggedIn = User.Identity?.IsAuthenticated == true;
        if (!IsLoggedIn)
            // If not logged in, show the standard welcome page.
            return Page();

        if (string.IsNullOrEmpty(ApiToken) || TokenHelper.IsTokenExpired(ApiToken))
        {
            // Token is missing or expired; sign out the user.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            IsLoggedIn = false;
            return Page();
        }

        // Load user profile from UserApi
        var userClient = _httpClientFactory.CreateClient("UserApi");
        userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        try
        {
            Profile = await userClient.GetFromJsonAsync<UserProfileSummary>("api/users/profile");
        }
        catch (UnauthorizedAccessException ex)
        {
            return new JsonResult(new { success = false, message = "Exception occurred: " + ex.Message });
        }

        // Load medicine uses from MedicineUses API
        var usesClient = _httpClientFactory.CreateClient("MedicineUsesApi");
        usesClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        List<MedicineUseDto> uses;
        try
        {
            uses = await usesClient.GetFromJsonAsync<List<MedicineUseDto>>("api/medicineuses") ?? [];
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = "Exception occurred: " + ex.Message });
        }

        // Build a summary list: calculate remaining days if DailyDose > 0
        MedicineUses = uses.Select(u => new MedicineUseSummary
        {
            MedicineName = u.MedicineName,
            DailyDose = u.DailyDose,
            StockAtHome = u.StockAtHome,
            RemainingDays = u.DailyDose > 0 ? (double)u.StockAtHome / u.DailyDose : null
        }).ToList();

        return Page();
    }

    // Handler for updating a medicine use.
    public async Task<IActionResult> OnPostUpdateMedicineUseAsync(int id, int? newDailyDose, int? newStockAtHome,
        bool addStock = false)
    {
        if (string.IsNullOrEmpty(ApiToken))
            return new JsonResult(new { success = false, message = "User not authenticated." });

        var client = _httpClientFactory.CreateClient("MedicineUsesApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);

        var updateRequest = new MedicineUseUpdateRequest
        {
            Id = id,
            NewDailyDose = newDailyDose,
            NewStockAtHome = newStockAtHome,
            AddStock = addStock
        };

        HttpResponseMessage response;
        try
        {
            response = await client.PutAsJsonAsync($"api/medicineuses/{id}", updateRequest);
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = "Exception occurred: " + ex.Message });
        }

        return response.IsSuccessStatusCode
            ? new JsonResult(new { success = true })
            : new JsonResult(new { success = false });
    }

    // Handler for deleting a medicine use.
    public async Task<IActionResult> OnPostDeleteMedicineUseAsync(int id)
    {
        if (string.IsNullOrEmpty(ApiToken))
            return new JsonResult(new { success = false, message = "User not authenticated." });

        var client = _httpClientFactory.CreateClient("MedicineUsesApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        HttpResponseMessage response;
        try
        {
            response = await client.DeleteAsync($"api/medicineuses/{id}");
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = "Exception occurred: " + ex.Message });
        }

        return response.IsSuccessStatusCode
            ? new JsonResult(new { success = true })
            : new JsonResult(new { success = false });
    }
}