using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioApi.Data;

namespace PortfolioApi.Tests;

public sealed class PortfolioApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static string RepositoryRoot { get; } = FindRepositoryRoot();
    public string ConnectionString { get; } = CreateTestConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Path.Combine(RepositoryRoot, "backend"));
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            // Keep the real controllers, services and repositories; only isolate the database.
            services.RemoveAll<PortfolioDbContext>();
            services.RemoveAll<DbContextOptions<PortfolioDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PortfolioDbContext>>();
            services.AddDbContext<PortfolioDbContext>(options => options.UseSqlServer(ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await DbInitializer.InitializeAsync(context);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        try
        {
            await using var scope = Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
            // The generated name below guarantees this is not the portfolio database.
            await context.Database.EnsureDeletedAsync();
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    private static string CreateTestConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(RepositoryRoot, "backend"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connection = new SqlConnectionStringBuilder(
            configuration["PORTFOLIO_TEST_CONNECTION"]
            ?? configuration.GetConnectionString("PortfolioDatabase")
            ?? throw new InvalidOperationException("No SQL Server connection is configured for the tests."))
        {
            InitialCatalog = $"WpfwPortfolioTests_{Guid.NewGuid():N}",
            Pooling = false
        };
        return connection.ConnectionString;
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "backend", "PortfolioApi.csproj")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not find the backend project for integration tests.");
    }
}
