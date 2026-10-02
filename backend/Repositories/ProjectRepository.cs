using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class ProjectRepository
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
     * Reads all projects from the temporary data. No parameters are required.
     * @returns {Project[]} The complete project list.
     */
    public Project[] GetAll()
    {
        return Projects;
    }

    /**
     * Searches the temporary data for a project with the requested id.
     * @param {int} id - The id to search for.
     * @returns {Project?} The matching project, or null if no match exists.
     */
    public Project? GetById(int id)
    {
        return Array.Find(Projects, project => project.Id == id);
    }
}
