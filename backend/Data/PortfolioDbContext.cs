using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;
using PortfolioApi.Security;

namespace PortfolioApi.Data;

public class PortfolioDbContext : IdentityDbContext<IdentityUser>
{
    /**
     * Receives the database configuration through dependency injection.
     * @param {DbContextOptions<PortfolioDbContext>} options - SQL Server connection and EF Core settings.
     * @returns {void} No return value, initializes the database context.
     */
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    // Each DbSet represents a table that EF Core can read and update.
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Keep seed identifiers stable so future migrations do not replace role memberships.
        builder.Entity<IdentityRole>().HasData(
            new { Id = "portfolio-guest", Name = PortfolioRoles.Guest, NormalizedName = "GAST" },
            new { Id = "portfolio-editor", Name = PortfolioRoles.Editor, NormalizedName = "EDITOR" },
            new { Id = "portfolio-admin", Name = PortfolioRoles.Admin, NormalizedName = "ADMIN" });
    }
}
