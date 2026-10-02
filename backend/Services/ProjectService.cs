using PortfolioApi.Dtos.Projects;
using PortfolioApi.Models;
using PortfolioApi.Repositories;

namespace PortfolioApi.Services;

public class ProjectService
{
    private readonly ProjectRepository _projectRepository;

    /**
     * Receives the project repository through dependency injection.
     * @param {ProjectRepository} projectRepository - Repository used to access project data.
     * @returns {void} No return value, initializes the service.
     */
    public ProjectService(ProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /**
     * Retrieves all projects and converts them to output DTOs. No parameters are required.
     * @returns {ProjectDto[]} The complete project list as DTOs.
     */
    public ProjectDto[] GetAll()
    {
        return _projectRepository.GetAll().Select(project => ToDto(project)).ToArray();
    }

    /**
     * Checks the requested id before retrieving a project from the repository.
     * @param {int} id - The project id, which must be greater than zero.
     * @returns {ProjectDto?} The project DTO, or null if the id is invalid or the project does not exist.
     */
    public ProjectDto? GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        var project = _projectRepository.GetById(id);

        if (project is null)
        {
            return null;
        }

        return ToDto(project);
    }

    /**
     * Converts validated input to a project, trims its text, and saves it through the repository.
     * @param {CreateProjectDto} dto - Project input that has passed the controller's DTO validation.
     * @returns {ProjectDto} The saved project with its id.
     */
    public ProjectDto Create(CreateProjectDto dto)
    {
        var project = new Project
        {
            Title = dto.Title.Trim(),
            Type = dto.Type.Trim(),
            Description = dto.Description.Trim()
        };

        _projectRepository.Create(project);
        return ToDto(project);
    }

    /**
     * Finds a project and replaces its editable fields with trimmed input.
     * @param {int} id - The project id, which must be greater than zero.
     * @param {UpdateProjectDto} dto - Project input that has passed the controller's DTO validation.
     * @returns {bool} True when the project is updated, or false if the id is invalid or the project does not exist.
     */
    public bool Update(int id, UpdateProjectDto dto)
    {
        if (id <= 0)
        {
            return false;
        }

        var project = _projectRepository.GetById(id);

        if (project is null)
        {
            return false;
        }

        project.Title = dto.Title.Trim();
        project.Type = dto.Type.Trim();
        project.Description = dto.Description.Trim();

        _projectRepository.Update(project);
        return true;
    }

    /**
     * Finds a project and asks the repository to delete it.
     * @param {int} id - The project id, which must be greater than zero.
     * @returns {bool} True when the project is deleted, or false if the id is invalid or the project does not exist.
     */
    public bool Delete(int id)
    {
        if (id <= 0)
        {
            return false;
        }

        var project = _projectRepository.GetById(id);

        if (project is null)
        {
            return false;
        }

        _projectRepository.Delete(project);
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
