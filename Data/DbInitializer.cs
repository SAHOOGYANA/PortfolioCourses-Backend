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
            Email = "admin@gyana.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@20041212")
        };

        context.AdminUsers.Add(admin);
        context.SaveChanges();
    }
}
