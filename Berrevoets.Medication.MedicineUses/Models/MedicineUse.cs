using System.ComponentModel.DataAnnotations;

namespace Berrevoets.Medication.MedicineUses.Models;

public class MedicineUse
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