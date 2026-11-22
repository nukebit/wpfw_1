using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Security;

namespace PortfolioApi.Tests;

public sealed class CrudEndpointTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Post_ReturnsCreatedDtoAndLocation_AndPersistsItem(string resource)
    {
        // Arrange
        using var client = await factory.CreateSignedInClientAsync(PortfolioRoles.Editor);

        // Act
        using var response = await client.PostAsJsonAsync($"/api/{resource}", Payload(resource, "  Created item  "));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var created = await ReadObjectAsync(response);
        Assert.True(created["id"]!.GetValue<int>() > 0);
        Assert.Equal("Created item", created["title"]!.GetValue<string>());

        using var getResponse = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await ReadObjectAsync(getResponse);
        Assert.True(JsonNode.DeepEquals(created, retrieved));

        var all = await client.GetFromJsonAsync<JsonArray>($"/api/{resource}");
        Assert.NotNull(all);
        Assert.Contains(all, item => item!["id"]!.GetValue<int>() == created["id"]!.GetValue<int>());
    }

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Put_ReturnsNoContent_AndPersistsChangedFields(string resource)
    {
        // Arrange
        using var client = await factory.CreateSignedInClientAsync(PortfolioRoles.Admin);
        var id = await CreateAsync(client, resource);
        var payload = Payload(resource, "  Updated item  ");
        if (resource == "blogposts") payload["publishedOn"] = "2026-10-29";

        // Act
        using var response = await client.PutAsJsonAsync($"/api/{resource}/{id}", payload);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("", await response.Content.ReadAsStringAsync());
        var retrieved = await client.GetFromJsonAsync<JsonObject>($"/api/{resource}/{id}");
        Assert.NotNull(retrieved);
        Assert.Equal(id, retrieved["id"]!.GetValue<int>());
        Assert.Equal("Updated item", retrieved["title"]!.GetValue<string>());
        if (resource == "blogposts") Assert.Equal("2026-10-29", retrieved["publishedOn"]!.GetValue<string>());
    }

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Delete_RemovesItem_AndSubsequentGetReturnsNotFound(string resource)
    {
        // Arrange
        using var client = await factory.CreateSignedInClientAsync(PortfolioRoles.Editor);
        var id = await CreateAsync(client, resource);

        // Act
        using var response = await client.DeleteAsync($"/api/{resource}/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var getResponse = await client.GetAsync($"/api/{resource}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Theory]
    [InlineData("projects", "POST")]
    [InlineData("projects", "PUT")]
    [InlineData("projects", "DELETE")]
    [InlineData("blogposts", "POST")]
    [InlineData("blogposts", "PUT")]
    [InlineData("blogposts", "DELETE")]
    public async Task Write_WithoutLoginOrAsGuest_ReturnsUnauthorizedOrForbidden(string resource, string method)
    {
        // Arrange
        using var anonymous = factory.CreateApiClient();
        using var guest = await factory.CreateSignedInClientAsync();
        using var anonymousRequest = CreateRequest(resource, method);
        using var guestRequest = CreateRequest(resource, method);

        // Act
        using var unauthorized = await anonymous.SendAsync(anonymousRequest);
        using var forbidden = await guest.SendAsync(guestRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Theory]
    [InlineData("projects", "GET")]
    [InlineData("projects", "PUT")]
    [InlineData("projects", "DELETE")]
    [InlineData("blogposts", "GET")]
    [InlineData("blogposts", "PUT")]
    [InlineData("blogposts", "DELETE")]
    public async Task UnknownId_ReturnsNotFoundProblemDetails(string resource, string method)
    {
        // Arrange
        using var client = method == "GET"
            ? factory.CreateApiClient()
            : await factory.CreateSignedInClientAsync(PortfolioRoles.Editor);
        using var request = CreateRequest(resource, method);

        // Act
        using var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
        Assert.Contains(int.MaxValue.ToString(), problem.Detail);
    }

    [Theory]
    [InlineData("projects", "POST")]
    [InlineData("projects", "PUT")]
    [InlineData("blogposts", "POST")]
    [InlineData("blogposts", "PUT")]
    public async Task MissingRequiredFields_ReturnsBadRequest_AndDoesNotChangeExistingItem(string resource, string method)
    {
        // Arrange
        using var client = await factory.CreateSignedInClientAsync(PortfolioRoles.Editor);
        var id = await CreateAsync(client, resource);
        var url = method == "POST" ? $"/api/{resource}" : $"/api/{resource}/{id}";
        using var request = new HttpRequestMessage(new HttpMethod(method), url)
        {
            Content = JsonContent.Create(new { })
        };

        // Act
        using var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Title", problem.Errors.Keys);
        if (resource == "blogposts") Assert.Contains("PublishedOn", problem.Errors.Keys);
        var existing = await client.GetFromJsonAsync<JsonObject>($"/api/{resource}/{id}");
        Assert.Equal("Test item", existing!["title"]!.GetValue<string>());
    }

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task TitleOverMaximumLength_ReturnsValidationError(string resource)
    {
        // Arrange
        using var client = await factory.CreateSignedInClientAsync(PortfolioRoles.Editor);

        // Act
        using var response = await client.PostAsJsonAsync($"/api/{resource}", Payload(resource, new string('x', 101)));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Title", problem.Errors.Keys);
    }

    private static HttpRequestMessage CreateRequest(string resource, string method)
    {
        var path = method == "POST" ? $"/api/{resource}" : $"/api/{resource}/{int.MaxValue}";
        return new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = method is "POST" or "PUT" ? JsonContent.Create(Payload(resource)) : null
        };
    }

    private static async Task<int> CreateAsync(HttpClient client, string resource)
    {
        using var response = await client.PostAsJsonAsync($"/api/{resource}", Payload(resource));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadObjectAsync(response))["id"]!.GetValue<int>();
    }

    private static JsonObject Payload(string resource, string title = "Test item") => resource == "projects"
        ? new JsonObject { ["title"] = title, ["type"] = "applicatie", ["description"] = "Test description" }
        : new JsonObject { ["title"] = title, ["category"] = "Testing", ["content"] = "Test content", ["publishedOn"] = "2026-10-01" };

    private static async Task<JsonObject> ReadObjectAsync(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(result);
        return result;
    }
}
