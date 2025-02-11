using Berrevoets.Medication.UserApi.Data;
using Berrevoets.Medication.UserApi.Models;
using Microsoft.AspNetCore.Identity;

namespace Berrevoets.Medication.UserApi;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        // Create a new scope to retrieve scoped services like the DbContext
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        // If there are any users, then assume the DB has been seeded
        if (context.Users.Any()) return;

        // Create an instance of PasswordHasher to hash the Admin password
        var passwordHasher = new PasswordHasher<User>();

        // Create the Admin user
        var adminUser = new User
        {
            Username = "Admin",
            Role = "Admin"
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin");

        // Add the Admin user to the DbContext and save changes
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}