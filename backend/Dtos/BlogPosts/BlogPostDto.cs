namespace PortfolioApi.Dtos.BlogPosts;

// Output returned by the API, separate from the EF Core database model.
public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public string Content { get; set; } = "";
    public DateOnly PublishedOn { get; set; }
}
