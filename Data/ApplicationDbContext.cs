using Microsoft.EntityFrameworkCore;
using PortfolioCourses.Api.Models;

namespace PortfolioCourses.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lecture> Lectures => Set<Lecture>();
    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectVideo> ProjectVideos => Set<ProjectVideo>();
    public DbSet<SiteAsset> SiteAssets => Set<SiteAsset>();
}
