using Berrevoets.Medication.MedicineCatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.MedicineCatalog.Data;

public class MedicineCatalogDbContext : DbContext
{
    public MedicineCatalogDbContext(DbContextOptions<MedicineCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Medicine> Medicines { get; set; }

    // Optional: Automatically set audit properties
    public override int SaveChanges()
    {
        UpdateAuditProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditProperties()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e is { Entity: Medicine, State: EntityState.Added or EntityState.Modified });
        foreach (var entry in entries)
        {
            var medicine = (Medicine)entry.Entity;
            medicine.LastUpdateDate = DateTime.UtcNow;
            if (entry.State == EntityState.Added) medicine.CreatedDate = DateTime.UtcNow;
        }
    }
}
