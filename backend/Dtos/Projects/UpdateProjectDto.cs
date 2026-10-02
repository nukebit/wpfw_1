using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos.Projects;

// Input for replacing a project's editable fields. The id comes from the URL.
public class UpdateProjectDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must be 100 characters or fewer.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Type is required.")]
    [StringLength(50, ErrorMessage = "Type must be 50 characters or fewer.")]
    public string Type { get; set; } = "";

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(2000, ErrorMessage = "Description must be 2000 characters or fewer.")]
    public string Description { get; set; } = "";
}
