using System.ComponentModel.DataAnnotations;
using Berrevoets.Medication.ServiceDefaults.Models;

namespace Berrevoets.Medication.UserApi.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
    public string Username { get; set; } = string.Empty;

    // Store the hashed password rather than the plain text
    public string PasswordHash { get; set; } = string.Empty;

    // You can also include a role for authorization purposes (default "User")
    public string Role { get; set; } = UserRole.User.ToString();

    [Required]
    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
    public string? PhoneNumber { get; set; }

    // Audit properties
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}