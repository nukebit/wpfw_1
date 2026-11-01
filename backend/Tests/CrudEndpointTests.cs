using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioApi.Tests;

public sealed class CrudEndpointTests(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Post_ReturnsCreatedDtoAndLocation_AndPersistsItem(string resource)
    {
        var response = await _client.PostAsJsonAsync($"/api/{resource}", Payload(resource, "  Created item  "));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var created = await ReadObjectAsync(response);
        Assert.True(created["id"]!.GetValue<int>() > 0);
        Assert.Equal("Created item", created["title"]!.GetValue<string>());

        var getResponse = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await ReadObjectAsync(getResponse);
        Assert.True(JsonNode.DeepEquals(created, retrieved));

        var all = await _client.GetFromJsonAsync<JsonArray>($"/api/{resource}");
        Assert.NotNull(all);
        Assert.Contains(all, item => item!["id"]!.GetValue<int>() == created["id"]!.GetValue<int>());
    }

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Put_ReturnsNoContent_AndPersistsChangedFields(string resource)
    {
        var id = await CreateAsync(resource);
        var payload = Payload(resource, "  Updated item  ");
        if (resource == "blogposts") payload["publishedOn"] = "2026-10-29";

        var response = await _client.PutAsJsonAsync($"/api/{resource}/{id}", payload);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("", await response.Content.ReadAsStringAsync());
        var retrieved = await _client.GetFromJsonAsync<JsonObject>($"/api/{resource}/{id}");
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
        var id = await CreateAsync(resource);

        var response = await _client.DeleteAsync($"/api/{resource}/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var getResponse = await _client.GetAsync($"/api/{resource}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
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
        using var request = new HttpRequestMessage(new HttpMethod(method), $"/api/{resource}/{int.MaxValue}");
        if (method == "PUT") request.Content = JsonContent.Create(Payload(resource));

        var response = await _client.SendAsync(request);

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
        var id = await CreateAsync(resource);
        var url = method == "POST" ? $"/api/{resource}" : $"/api/{resource}/{id}";
        using var request = new HttpRequestMessage(new HttpMethod(method), url)
        {
            Content = JsonContent.Create(new { })
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Title", problem.Errors.Keys);
        if (resource == "blogposts") Assert.Contains("PublishedOn", problem.Errors.Keys);
        var existing = await _client.GetFromJsonAsync<JsonObject>($"/api/{resource}/{id}");
        Assert.Equal("Test item", existing!["title"]!.GetValue<string>());
    }

    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task TitleOverMaximumLength_ReturnsValidationError(string resource)
    {
        var response = await _client.PostAsJsonAsync($"/api/{resource}", Payload(resource, new string('x', 101)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Title", problem.Errors.Keys);
    }

    private async Task<int> CreateAsync(string resource)
    {
        var response = await _client.PostAsJsonAsync($"/api/{resource}", Payload(resource));
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
