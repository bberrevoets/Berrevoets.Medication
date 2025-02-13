using System.ComponentModel.DataAnnotations;

namespace Berrevoets.Medication.MedicineCatalog.Models;

public class Medicine
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Medicine name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "Manufacturer cannot be longer than 100 characters.")]
    public string? Manufacturer { get; set; }

    // Example fields for dosage information, stock etc.
    public int? StandardDose { get; set; }

    public int? Stock { get; set; }

    // Audit properties
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}