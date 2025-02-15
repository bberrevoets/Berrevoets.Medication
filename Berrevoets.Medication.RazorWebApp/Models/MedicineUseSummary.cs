namespace Berrevoets.Medication.RazorWebApp.Models;

// DTO returned from the MedicineUses API.
public class MedicineUseSummary
{
    public int Id { get; set; }
    public string MedicineName { get; set; } = "";
    public int DailyDose { get; set; }
    public int StockAtHome { get; set; }
    public double? RemainingDays { get; set; }
}