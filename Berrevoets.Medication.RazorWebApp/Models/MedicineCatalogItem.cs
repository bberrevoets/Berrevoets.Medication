namespace Berrevoets.Medication.RazorWebApp.Models;

public class MedicineCatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public int Stock { get; set; }
}