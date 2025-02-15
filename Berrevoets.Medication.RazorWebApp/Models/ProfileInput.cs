using System.ComponentModel.DataAnnotations;

namespace Berrevoets.Medication.RazorWebApp.Models;

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