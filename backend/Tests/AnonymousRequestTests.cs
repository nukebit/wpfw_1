using System.Net;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioApi.Tests;

// These requests must be rejected before any database-dependent action runs.
public sealed class AnonymousRequestTests : IClassFixture<AnonymousApiFactory>
{
    private readonly AnonymousApiFactory factory;
    public AnonymousRequestTests(AnonymousApiFactory factory) => this.factory = factory;

    [Theory]
    [InlineData("GET", "/api/auth/me")]
    [InlineData("POST", "/api/auth/logout")]
    [InlineData("POST", "/api/projects")]
    [InlineData("PUT", "/api/projects/1")]
    [InlineData("DELETE", "/api/projects/1")]
    [InlineData("POST", "/api/blogposts")]
    [InlineData("PUT", "/api/blogposts/1")]
    [InlineData("DELETE", "/api/blogposts/1")]
    public async Task ProtectedRoute_WithoutCookie_Returns401InsteadOfRedirect(string method, string path)
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        using var request = new HttpRequestMessage(new HttpMethod(method), path);
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(response.Headers.Location);
    }
}

public sealed class AnonymousApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Path.Combine(PortfolioApiFactory.RepositoryRoot, "backend"));
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
            services.AddDataProtection().UseEphemeralDataProtectionProvider());
    }
}
