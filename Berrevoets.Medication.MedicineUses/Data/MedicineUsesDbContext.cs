using Berrevoets.Medication.MedicineUses.Models;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.MedicineUses.Data;

public class MedicineUsesDbContext : DbContext
{
    public MedicineUsesDbContext(DbContextOptions<MedicineUsesDbContext> options)
        : base(options)
    {
    }

    public DbSet<MedicineUse> MedicineUses { get; set; }

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
            .Where(e => e.Entity is MedicineUse && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var record = (MedicineUse)entry.Entity;
            record.LastUpdateDate = DateTime.UtcNow;
            if (entry.State == EntityState.Added) record.CreatedDate = DateTime.UtcNow;
        }
    }
}