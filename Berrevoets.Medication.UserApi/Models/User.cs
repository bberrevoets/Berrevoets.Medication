namespace Berrevoets.Medication.UserApi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    // Store the hashed password rather than the plain text
    public string PasswordHash { get; set; } = string.Empty;
    // You can also include a role for authorization purposes (default "User")
    public string Role { get; set; } = "User";
}