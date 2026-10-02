using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos.BlogPosts;

// Input for a new blog post. SQL Server generates the id.
public class CreateBlogPostDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must be 100 characters or fewer.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category must be 50 characters or fewer.")]
    public string Category { get; set; } = "";

    [Required(ErrorMessage = "Content is required.")]
    [StringLength(10000, ErrorMessage = "Content must be 10000 characters or fewer.")]
    public string Content { get; set; } = "";

    // Nullable allows Required to detect a missing date in the JSON body.
    [Required(ErrorMessage = "Publication date is required.")]
    public DateOnly? PublishedOn { get; set; }
}
