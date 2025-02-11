using Berrevoets.Medication.UserApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.UserApi.Data;

public class UserDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}