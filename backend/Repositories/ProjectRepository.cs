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

    /**
     * Inserts a new project into SQL Server and saves its generated id on the model.
     * @param {Project} project - The new project model, with an id of zero before it is saved.
     * @returns {void} No return value, saves the project in the database.
     */
    public void Create(Project project)
    {
        _context.Projects.Add(project);
        _context.SaveChanges();
    }

    /**
     * Saves the changed fields of an existing project in SQL Server.
     * @param {Project} project - An existing project model with its id and updated fields.
     * @returns {void} No return value, saves the changes in the database.
     */
    public void Update(Project project)
    {
        _context.Projects.Update(project);
        _context.SaveChanges();
    }

    /**
     * Removes an existing project from SQL Server.
     * @param {Project} project - The existing project model to delete.
     * @returns {void} No return value, deletes the project from the database.
     */
    public void Delete(Project project)
    {
        _context.Projects.Remove(project);
        _context.SaveChanges();
    }
}
