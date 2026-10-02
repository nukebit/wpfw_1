using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    // temp data
    private static readonly Project[] Projects =
    {
        new Project
        {
            Id = 1,
            Title = "Task management platform",
            Type = "applicatie",
            Description = "Een applicatie voor het maken van projecten, verdelen van taken en volgen van de voortgang."
        },
        new Project
        {
            Id = 2,
            Title = "Event booking application",
            Type = "applicatie",
            Description = "Een applicatie waarin gebruikers evenementen bekijken, plaatsen reserveren en boekingen beheren."
        },
        new Project
        {
            Id = 3,
            Title = "Inventory dashboard",
            Type = "dashboard",
            Description = "Een dashboard voor productbeheer, voorraadupdates en meldingen bij lage voorraad."
        }
    };

    /**
     * Returns all projects from the temporary data. No parameters are required.
     * @returns {ActionResult<Project[]>} HTTP 200 with the project list as JSON.
     */
    [HttpGet]
    public ActionResult<Project[]> GetAll()
    {
        return Ok(Projects);
    }

    /**
     * Looks up one project in the temporary data using its id.
     * @param {int} id - The id of the requested project.
     * @returns {ActionResult<Project>} HTTP 200 with the project as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> GetById(int id)
    {
        var project = Array.Find(Projects, project => project.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }
}
