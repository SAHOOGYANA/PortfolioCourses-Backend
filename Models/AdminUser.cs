namespace PortfolioCourses.Api.Models;

public class AdminUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}
