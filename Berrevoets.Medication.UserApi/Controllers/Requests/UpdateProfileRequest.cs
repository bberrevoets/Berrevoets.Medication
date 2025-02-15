using System.ComponentModel.DataAnnotations;

namespace Berrevoets.Medication.UserApi.Controllers.Requests
{
    public class UpdateProfileRequest
    {
        // Optional: if the user wants to change the username
        public string? Username { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; } = string.Empty;
        
        // Optional phone number
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        public string? PhoneNumber { get; set; }
    }
}