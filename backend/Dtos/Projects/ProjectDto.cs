namespace PortfolioApi.Dtos.Projects;

// Output returned by the API, separate from the EF Core database model.
public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
}
