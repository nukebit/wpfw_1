using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public class BlogPostRepository
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
     * @returns {BlogPost[]} The complete blog post list, or an empty array if the table is empty.
     */
    public BlogPost[] GetAll()
    {
        return _context.BlogPosts.AsNoTracking().OrderBy(blogPost => blogPost.Id).ToArray();
    }

    /**
     * Searches SQL Server for a blog post with the requested id.
     * @param {int} id - The id to search for.
     * @returns {BlogPost?} The matching blog post, or null if no match exists.
     */
    public BlogPost? GetById(int id)
    {
        return _context.BlogPosts.AsNoTracking().FirstOrDefault(blogPost => blogPost.Id == id);
    }
}
