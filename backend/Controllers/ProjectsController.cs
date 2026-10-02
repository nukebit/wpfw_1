using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;

    /**
     * Receives the project service through dependency injection.
     * @param {ProjectService} projectService - Service used to retrieve projects.
     * @returns {void} No return value, initializes the controller.
     */
    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    /**
     * Requests all projects from the service. No parameters are required.
     * @returns {ActionResult<Project[]>} HTTP 200 with the project list as JSON.
     */
    [HttpGet]
    public ActionResult<Project[]> GetAll()
    {
        return Ok(_projectService.GetAll());
    }

    /**
     * Requests one project from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested project.
     * @returns {ActionResult<Project>} HTTP 200 with the project as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> GetById(int id)
    {
        var project = _projectService.GetById(id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }
}
