using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Berrevoets.Medication.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Berrevoets.Medication.RazorWebApp.Pages;

[Authorize]
public class CatalogModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CatalogModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<MedicineCatalogItem> Medicines { get; set; } = new();

    // The token is now stored in a cookie-based user claim.
    // We'll extract it from User.Claims (e.g. claim type "Token").
    public string? ApiToken { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity.IsAuthenticated) return RedirectToPage("Login");

        // Extract the token from the authenticated user's claims.
        ApiToken = User.FindFirst("Token")?.Value;
        var catalogClient  = _httpClientFactory.CreateClient("MedicineCatalogApi");
        if (!string.IsNullOrEmpty(ApiToken))
            catalogClient .DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);

        try
        {
            var allMedicines = await catalogClient.GetFromJsonAsync<List<MedicineCatalogItem>>("api/medicines")
                               ?? new List<MedicineCatalogItem>();

            // Now call MedicineUses API to get the list of medicine IDs already added by the user.
            var usesClient = _httpClientFactory.CreateClient("MedicineUsesApi");
            if (!string.IsNullOrEmpty(ApiToken))
            {
                usesClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
            }

            // Assume your API returns a list of objects that contain MedicineCatalogId.
            var userUses = await usesClient.GetFromJsonAsync<List<MedicineUseDto>>("api/medicineuses")
                           ?? new List<MedicineUseDto>();

            var addedIds = userUses.Select(u => u.MedicineCatalogId).ToHashSet();

            // Filter out medicines that the user has already added.
            Medicines = allMedicines.Where(m => !addedIds.Contains(m.Id)).ToList();
        }
        catch
        {
            Medicines = new List<MedicineCatalogItem>();
        }

        return Page();
    }

    // AJAX handler for adding a medicine to the user's list.
    public async Task<IActionResult> OnPostAddMedicineAsync(int medicineCatalogId, string medicineName)
    {
        ApiToken = User.FindFirst("Token")?.Value;
        if (string.IsNullOrEmpty(ApiToken))
            return new JsonResult(new { success = false, message = "User not authenticated." });

        var client = _httpClientFactory.CreateClient("MedicineUsesApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        var userId = JwtHelper.GetUserIdFromToken(ApiToken);
        // Build a request body. In a real application, include more information as needed.
        var body = new
        {
            MedicineCatalogId = medicineCatalogId,
            MedicineName = medicineName,
            UserId = userId
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/medicineuses", body);
            if (response.IsSuccessStatusCode) return new JsonResult(new { success = true });

            return new JsonResult(new { success = false, message = "API call failed." });
        }
        catch
        {
            return new JsonResult(new { success = false, message = "Exception occurred." });
        }
    }
}

// This DTO should match what the MedicineUses API returns.
public class MedicineUseDto
{
    public int Id { get; set; }

    [Required] [StringLength(100)] public string MedicineName { get; set; } = string.Empty;

    [Required] public int? MedicineCatalogId { get; set; }

    [Required] public string UserId { get; set; } = string.Empty;

    public int DailyDose { get; set; }

    public int StockAtHome { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}

public class MedicineCatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public int Stock { get; set; }
}