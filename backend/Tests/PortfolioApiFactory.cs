using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Auth;

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
            services.AddDataProtection().UseEphemeralDataProtectionProvider();

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

    public HttpClient CreateApiClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        // Match the Secure cookie policy used outside the local HTTP development profile.
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false
    });

    public static string CreateTestPassword() => $"Test-{Guid.NewGuid():N}9!";

    public async Task<HttpClient> CreateSignedInClientAsync(string? extraRole = null)
    {
        var client = CreateApiClient();
        var email = $"test-{Guid.NewGuid():N}@example.test";
        var password = CreateTestPassword();
        try
        {
            using var registration = await client.PostAsJsonAsync("/api/auth/register",
                new RegisterRequest { Email = email, Password = password });
            Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

            if (extraRole is not null)
            {
                await using var scope = Services.CreateAsyncScope();
                var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var user = await users.FindByEmailAsync(email);
                Assert.NotNull(user);
                var assigned = await users.AddToRoleAsync(user, extraRole);
                Assert.True(assigned.Succeeded);
            }

            using var login = await client.PostAsJsonAsync("/api/auth/login",
                new LoginRequest { Email = email, Password = password });
            Assert.Equal(HttpStatusCode.NoContent, login.StatusCode);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
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
