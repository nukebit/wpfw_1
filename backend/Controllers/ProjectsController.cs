using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Projects;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    /**
     * Receives the project service through dependency injection.
     * @param {IProjectService} projectService - Service used to retrieve projects.
     * @returns {void} No return value, initializes the controller.
     */
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /**
     * Requests all projects as output DTOs. No parameters are required.
     * @returns {Task<ActionResult<ProjectDto[]>>} HTTP 200 with the project list as JSON.
     */
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto[]>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _projectService.GetAllAsync(cancellationToken));
    }

    /**
     * Requests one project from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested project.
     * @returns {Task<ActionResult<ProjectDto>>} HTTP 200 with the project DTO, or HTTP 404 with error details if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found.",
                detail: $"No project exists with id {id}.");
        }

        return Ok(project);
    }

    /**
     * Creates a project from the validated JSON request body.
     * @param {CreateProjectDto} dto - Required title, type, and description within the DTO length limits.
     * @returns {Task<ActionResult<ProjectDto>>} HTTP 201 with the created project and its URL, or HTTP 400 for invalid input.
     */
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    /**
     * Replaces the editable fields of an existing project using a validated JSON request body.
     * @param {int} id - The id of the project to update, taken from the URL.
     * @param {UpdateProjectDto} dto - Required title, type, and description within the DTO length limits.
     * @returns {Task<IActionResult>} HTTP 204 on success, HTTP 400 for invalid input, or HTTP 404 with error details for a missing project.
     */
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        var updated = await _projectService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found.",
                detail: $"No project exists with id {id}.");
        }

        return NoContent();
    }

    /**
     * Deletes a project with the requested id.
     * @param {int} id - The id of the project to delete, taken from the URL.
     * @returns {Task<IActionResult>} HTTP 204 on success, or HTTP 404 with error details for a missing project.
     */
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _projectService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Project not found.",
                detail: $"No project exists with id {id}.");
        }

        return NoContent();
    }
}
