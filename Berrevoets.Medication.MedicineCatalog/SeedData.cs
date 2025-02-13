using Berrevoets.Medication.MedicineCatalog.Data;
using Berrevoets.Medication.MedicineCatalog.Models;

using Bogus;
using Bogus.Healthcare;

namespace Berrevoets.Medication.MedicineCatalog;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MedicineCatalogDbContext>();

        if (context.Medicines.Any()) return;

        var medicineFaker = new Faker<Medicine>()
            .RuleFor(m => m.Name, f => f.Drugs().DosageForm())
            .RuleFor(m => m.Description, f => f.Lorem.Sentence())
            .RuleFor(m => m.Manufacturer, f => f.Company.CompanyName())
            .RuleFor(m => m.StandardDose, f => f.Random.Number(1, 4))
            .RuleFor(m => m.Stock, f => f.Random.Number(10, 100));

        var medicines = medicineFaker.Generate(50);

        context.Medicines.AddRange(medicines);
        context.SaveChanges();
    }
}