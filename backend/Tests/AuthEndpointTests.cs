using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Dtos.Auth;
using PortfolioApi.Security;

namespace PortfolioApi.Tests;

public sealed class AuthEndpointTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    [Fact]
    public async Task Me_WithoutLogin_ReturnsUnauthorized()
    {
        using var client = factory.CreateApiClient();
        using var response = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(PortfolioRoles.Editor)]
    [InlineData(PortfolioRoles.Admin)]
    public async Task Me_AfterLoginAndReload_ReturnsCurrentRoles_AndLogoutEndsSession(string? role)
    {
        using var client = await factory.CreateSignedInClientAsync(role);
        // Calling the endpoint again models the session lookup after a page reload.
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var response = await client.GetAsync("/api/auth/me");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<CurrentUserResponse>();
            Assert.NotNull(user);
            Assert.Contains(PortfolioRoles.Guest, user.Roles);
            if (role is not null) Assert.Contains(role, user.Roles);
            Assert.EndsWith("@example.test", user.Email);
        }

        using var logout = await client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        using var afterLogout = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, afterLogout.StatusCode);
        using var write = await client.PostAsJsonAsync("/api/projects",
            new { Title = "Unauthorized", Type = "Web", Description = "Must not be saved" });
        Assert.Equal(HttpStatusCode.Unauthorized, write.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutLogin_ReturnsUnauthorized()
    {
        using var client = factory.CreateApiClient();
        using var response = await client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed record CurrentUserResponse(string Id, string Email, string[] Roles);

    [Fact]
    public async Task Register_WithRequestedAdminRole_CreatesGuestOnly()
    {
        // Arrange
        using var client = factory.CreateApiClient();
        var email = $"guest-{Guid.NewGuid():N}@example.test";
        var request = new { Email = email, Password = PortfolioApiFactory.CreateTestPassword(), Role = PortfolioRoles.Admin };

        // Act
        using var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        await using var scope = factory.Services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.Equal(new[] { PortfolioRoles.Guest }, await users.GetRolesAsync(user));
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateApiClient();
        var request = new RegisterRequest
        {
            Email = $"duplicate-{Guid.NewGuid():N}@example.test",
            Password = PortfolioApiFactory.CreateTestPassword()
        };
        using var registration = await client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        // Act
        using var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("invalid-email", false)]
    [InlineData("guest@example.test", true)]
    public async Task Register_WithInvalidInput_ReturnsBadRequest(string email, bool weakPassword)
    {
        // Arrange
        using var client = factory.CreateApiClient();
        var request = new RegisterRequest
        {
            Email = email,
            Password = weakPassword ? "short" : PortfolioApiFactory.CreateTestPassword()
        };

        // Act
        using var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Login_WithValidOrInvalidPassword_OnlyIssuesCookieForValidCredentials(bool correctPassword)
    {
        // Arrange
        using var client = factory.CreateApiClient();
        var email = $"login-{Guid.NewGuid():N}@example.test";
        var password = PortfolioApiFactory.CreateTestPassword();
        using var registration = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        // Act
        using var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Email = email, Password = correctPassword ? password : PortfolioApiFactory.CreateTestPassword() });

        // Assert
        Assert.Equal(correctPassword ? HttpStatusCode.NoContent : HttpStatusCode.Unauthorized, response.StatusCode);
        if (correctPassword)
        {
            Assert.Contains(response.Headers.GetValues("Set-Cookie"), value =>
                value.StartsWith("Portfolio.Auth=", StringComparison.Ordinal) &&
                value.Contains("httponly", StringComparison.OrdinalIgnoreCase) &&
                value.Contains("secure", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            Assert.False(response.Headers.Contains("Set-Cookie"));
        }
    }
}
