using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Tests;

public sealed class MigrationTests
{
    [Fact]
    public async Task Migrations_CreateFreshDatabase_AndCanBeAppliedAgain()
    {
        await using var factory = new PortfolioApiFactory();
        await using var context = CreateContext(factory);
        try
        {
            await DbInitializer.InitializeAsync(context);
            await DbInitializer.InitializeAsync(context);

            Assert.Single(await context.Database.GetAppliedMigrationsAsync());
            Assert.Empty(await context.Database.GetPendingMigrationsAsync());
            Assert.False(context.Database.HasPendingModelChanges());
            Assert.Equal(3, await context.Projects.CountAsync());
            Assert.Equal(3, await context.BlogPosts.CountAsync());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Baseline_AdoptsExistingSchema_WithoutChangingData_AndCanBeRunAgain()
    {
        await using var factory = new PortfolioApiFactory();
        await using var context = CreateContext(factory);
        try
        {
            await context.Database.MigrateAsync();
            var project = new Project { Title = "Keep project", Type = "dashboard", Description = "Existing data" };
            var blogPost = new BlogPost { Title = "Keep blog", Category = "Testing", Content = "Existing content", PublishedOn = new DateOnly(2026, 10, 1) };
            context.AddRange(project, blogPost);
            await context.SaveChangesAsync();

            // This reproduces the old database: normal tables, but no migration history.
            await context.Database.ExecuteSqlRawAsync("DROP TABLE [__EFMigrationsHistory]");
            var baseline = await ReadBaselineAsync();
            await context.Database.ExecuteSqlRawAsync(baseline);
            await context.Database.MigrateAsync();
            await context.Database.ExecuteSqlRawAsync(baseline);

            context.ChangeTracker.Clear();
            var savedProject = await context.Projects.SingleAsync();
            var savedBlogPost = await context.BlogPosts.SingleAsync();
            Assert.Equal(project.Id, savedProject.Id);
            Assert.Equal("Keep project", savedProject.Title);
            Assert.Equal("dashboard", savedProject.Type);
            Assert.Equal("Existing data", savedProject.Description);
            Assert.Equal(blogPost.Id, savedBlogPost.Id);
            Assert.Equal("Keep blog", savedBlogPost.Title);
            Assert.Equal("Testing", savedBlogPost.Category);
            Assert.Equal("Existing content", savedBlogPost.Content);
            Assert.Equal(blogPost.PublishedOn, savedBlogPost.PublishedOn);
            Assert.Single(await context.Database.GetAppliedMigrationsAsync());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync();
        }
    }

    [Theory]
    [InlineData("ALTER TABLE [Projects] ADD [Unexpected] int NULL")]
    [InlineData("ALTER TABLE [Projects] ALTER COLUMN [Title] nvarchar(100) NOT NULL")]
    public async Task Baseline_RejectsMismatchingSchema_WithoutRecordingMigration(string changeSchema)
    {
        await using var factory = new PortfolioApiFactory();
        await using var context = CreateContext(factory);
        try
        {
            await context.Database.MigrateAsync();
            await context.Database.ExecuteSqlRawAsync("DROP TABLE [__EFMigrationsHistory]");
            await context.Database.ExecuteSqlRawAsync(changeSchema);
            var baseline = await ReadBaselineAsync();

            var exception = await Assert.ThrowsAsync<SqlException>(() => context.Database.ExecuteSqlRawAsync(baseline));

            Assert.Equal(50001, exception.Number);
            var historyTables = await context.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) AS [Value] FROM sys.tables WHERE [name] = N'__EFMigrationsHistory'").SingleAsync();
            Assert.Equal(0, historyTables);
            Assert.True(await context.Database.CanConnectAsync());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync();
        }
    }

    private static PortfolioDbContext CreateContext(PortfolioApiFactory factory) =>
        new(new DbContextOptionsBuilder<PortfolioDbContext>().UseSqlServer(factory.ConnectionString).Options);

    private static Task<string> ReadBaselineAsync() => File.ReadAllTextAsync(
        Path.Combine(PortfolioApiFactory.RepositoryRoot, "backend", "Data", "BaselineExistingDatabase.sql"));
}
