using Berrevoets.Medication.UserApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.UserApi.Data;

public class UserDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

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
            .Where(e => e is { Entity: User, State: EntityState.Added or EntityState.Modified });

        foreach (var entry in entries)
        {
            var user = (User)entry.Entity;
            user.LastUpdateDate = DateTime.UtcNow;
            if (entry.State == EntityState.Added) user.CreatedDate = DateTime.UtcNow;
        }
    }
}