namespace Berrevoets.Medication.RazorWebApp.Pages;

public class UserProfileSummary
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public string Role { get; set; } = "";
}