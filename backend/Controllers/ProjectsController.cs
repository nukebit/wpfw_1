using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Projects;
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
     * Requests all projects as output DTOs. No parameters are required.
     * @returns {ActionResult<ProjectDto[]>} HTTP 200 with the project list as JSON.
     */
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ProjectDto[]> GetAll()
    {
        return Ok(_projectService.GetAll());
    }

    /**
     * Requests one project from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested project.
     * @returns {ActionResult<ProjectDto>} HTTP 200 with the project DTO as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProjectDto> GetById(int id)
    {
        var project = _projectService.GetById(id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    /**
     * Creates a project from the validated JSON request body.
     * @param {CreateProjectDto} dto - Required title, type, and description within the DTO length limits.
     * @returns {ActionResult<ProjectDto>} HTTP 201 with the created project and its URL, or HTTP 400 for invalid input.
     */
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<ProjectDto> Create([FromBody] CreateProjectDto dto)
    {
        var project = _projectService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    /**
     * Replaces the editable fields of an existing project using a validated JSON request body.
     * @param {int} id - The id of the project to update, taken from the URL.
     * @param {UpdateProjectDto} dto - Required title, type, and description within the DTO length limits.
     * @returns {IActionResult} HTTP 204 on success, HTTP 400 for invalid input, or HTTP 404 if the project does not exist.
     */
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateProjectDto dto)
    {
        var updated = _projectService.Update(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /**
     * Deletes a project with the requested id.
     * @param {int} id - The id of the project to delete, taken from the URL.
     * @returns {IActionResult} HTTP 204 on success, or HTTP 404 if the project does not exist.
     */
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var deleted = _projectService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
