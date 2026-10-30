using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly PortfolioDbContext _context;

    /**
     * Receives the database context through dependency injection.
     * @param {PortfolioDbContext} context - EF Core context used to access the blog posts table.
     * @returns {void} No return value, initializes the repository.
     */
    public BlogPostRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    /**
     * Reads all blog posts from SQL Server in id order. No parameters are required.
     * @returns {Task<IReadOnlyList<BlogPost>>} The complete blog post list, or an empty list if the table is empty.
     */
    public async Task<IReadOnlyList<BlogPost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts.AsNoTracking().OrderBy(blogPost => blogPost.Id).ToListAsync(cancellationToken);
    }

    /**
     * Searches SQL Server for a blog post with the requested id.
     * @param {int} id - The id to search for.
     * @returns {Task<BlogPost?>} The matching blog post, or null if no match exists.
     */
    public async Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(blogPost => blogPost.Id == id, cancellationToken);
    }

    /**
     * Inserts a new blog post into SQL Server and saves its generated id on the model.
     * @param {BlogPost} blogPost - The new blog post model, with an id of zero before it is saved.
     * @returns {Task} No return value, saves the blog post in the database.
     */
    public async Task CreateAsync(BlogPost blogPost, CancellationToken cancellationToken = default)
    {
        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /**
     * Saves the changed fields of an existing blog post in SQL Server.
     * @param {BlogPost} blogPost - An existing blog post model with its id and updated fields.
     * @returns {Task} No return value, saves the changes in the database.
     */
    public async Task UpdateAsync(BlogPost blogPost, CancellationToken cancellationToken = default)
    {
        _context.BlogPosts.Update(blogPost);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /**
     * Removes an existing blog post from SQL Server.
     * @param {BlogPost} blogPost - The existing blog post model to delete.
     * @returns {Task} No return value, deletes the blog post from the database.
     */
    public async Task DeleteAsync(BlogPost blogPost, CancellationToken cancellationToken = default)
    {
        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
