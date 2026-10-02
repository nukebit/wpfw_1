using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class ProjectRepository
{
    private readonly PortfolioDbContext _context;

    /**
     * Receives the database context through dependency injection.
     * @param {PortfolioDbContext} context - EF Core context used to access the projects table.
     * @returns {void} No return value, initializes the repository.
     */
    public ProjectRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    /**
     * Reads all projects from SQL Server in id order. No parameters are required.
     * @returns {Project[]} The complete project list, or an empty array if the table is empty.
     */
    public Project[] GetAll()
    {
        return _context.Projects.AsNoTracking().OrderBy(project => project.Id).ToArray();
    }

    /**
     * Searches SQL Server for a project with the requested id.
     * @param {int} id - The id to search for.
     * @returns {Project?} The matching project, or null if no match exists.
     */
    public Project? GetById(int id)
    {
        return _context.Projects.AsNoTracking().FirstOrDefault(project => project.Id == id);
    }
}
