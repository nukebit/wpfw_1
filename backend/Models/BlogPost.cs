namespace PortfolioApi.Models;

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public string Content { get; set; } = "";
    public DateOnly PublishedOn { get; set; }
}
