using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class ProjectRepository : IProjectRepository
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
     * @returns {Task<IReadOnlyList<Project>>} The complete project list, or an empty list if the table is empty.
     */
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects.AsNoTracking().OrderBy(project => project.Id).ToListAsync(cancellationToken);
    }

    /**
     * Searches SQL Server for a project with the requested id.
     * @param {int} id - The id to search for.
     * @returns {Task<Project?>} The matching project, or null if no match exists.
     */
    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects.AsNoTracking().FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    /**
     * Inserts a new project into SQL Server and saves its generated id on the model.
     * @param {Project} project - The new project model, with an id of zero before it is saved.
     * @returns {Task} No return value, saves the project in the database.
     */
    public async Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /**
     * Saves the changed fields of an existing project in SQL Server.
     * @param {Project} project - An existing project model with its id and updated fields.
     * @returns {Task} No return value, saves the changes in the database.
     */
    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /**
     * Removes an existing project from SQL Server.
     * @param {Project} project - The existing project model to delete.
     * @returns {Task} No return value, deletes the project from the database.
     */
    public async Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
