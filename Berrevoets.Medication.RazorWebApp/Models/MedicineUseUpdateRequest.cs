namespace Berrevoets.Medication.RazorWebApp.Models;

public class MedicineUseUpdateRequest
{
    public int Id { get; set; }
    public int? NewDailyDose { get; set; }
    public int? NewStockAtHome { get; set; }
    public bool AddStock { get; set; } = false;
}