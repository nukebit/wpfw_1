using PortfolioApi.Dtos.Projects;
using PortfolioApi.Models;
using PortfolioApi.Repositories;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    /**
     * Receives the project repository through dependency injection.
     * @param {IProjectRepository} projectRepository - Repository used to access project data.
     * @returns {void} No return value, initializes the service.
     */
    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /**
     * Retrieves all projects and converts them to output DTOs. No parameters are required.
     * @returns {Task<ProjectDto[]>} The complete project list as DTOs.
     */
    public async Task<ProjectDto[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _projectRepository.GetAllAsync(cancellationToken);
        return items.Select(ToDto).ToArray();
    }

    /**
     * Checks the requested id before retrieving a project from the repository.
     * @param {int} id - The project id, which must be greater than zero.
     * @returns {Task<ProjectDto?>} The project DTO, or null if the id is invalid or the project does not exist.
     */
    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return null;
        }

        return ToDto(project);
    }

    /**
     * Converts validated input to a project, trims its text, and saves it through the repository.
     * @param {CreateProjectDto} dto - Project input that has passed the controller's DTO validation.
     * @returns {Task<ProjectDto>} The saved project with its id.
     */
    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Title = dto.Title.Trim(),
            Type = dto.Type.Trim(),
            Description = dto.Description.Trim()
        };

        await _projectRepository.CreateAsync(project, cancellationToken);
        return ToDto(project);
    }

    /**
     * Finds a project and replaces its editable fields with trimmed input.
     * @param {int} id - The project id, which must be greater than zero.
     * @param {UpdateProjectDto} dto - Project input that has passed the controller's DTO validation.
     * @returns {Task<bool>} True when the project is updated, or false if the id is invalid or the project does not exist.
     */
    public async Task<bool> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return false;
        }

        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return false;
        }

        project.Title = dto.Title.Trim();
        project.Type = dto.Type.Trim();
        project.Description = dto.Description.Trim();

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return true;
    }

    /**
     * Finds a project and asks the repository to delete it.
     * @param {int} id - The project id, which must be greater than zero.
     * @returns {Task<bool>} True when the project is deleted, or false if the id is invalid or the project does not exist.
     */
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return false;
        }

        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return false;
        }

        await _projectRepository.DeleteAsync(project, cancellationToken);
        return true;
    }

    /**
     * Copies the public project fields from a database model to an output DTO.
     * @param {Project} project - The project model retrieved from or saved to the database.
     * @returns {ProjectDto} The project data returned to API clients.
     */
    private static ProjectDto ToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Type = project.Type,
            Description = project.Description
        };
    }
}
