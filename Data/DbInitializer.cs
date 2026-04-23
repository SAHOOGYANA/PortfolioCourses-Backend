using PortfolioCourses.Api.Models;
using BCrypt.Net;

namespace PortfolioCourses.Api.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.AdminUsers.Any())
            return; // Admin already exists

        var admin = new AdminUser
        {
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        };

        context.AdminUsers.Add(admin);
        context.SaveChanges();
    }
}
