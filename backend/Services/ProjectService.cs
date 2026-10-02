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
     * Retrieves all projects from the repository. No parameters are required.
     * @returns {Project[]} The complete project list.
     */
    public Project[] GetAll()
    {
        return _projectRepository.GetAll();
    }

    /**
     * Checks the requested id before retrieving a project from the repository.
     * @param {int} id - The project id, which must be greater than zero.
     * @returns {Project?} The project, or null if the id is invalid or the project does not exist.
     */
    public Project? GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return _projectRepository.GetById(id);
    }
}
