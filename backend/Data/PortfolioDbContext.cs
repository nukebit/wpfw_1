using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Data;

public class PortfolioDbContext : DbContext
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
}
