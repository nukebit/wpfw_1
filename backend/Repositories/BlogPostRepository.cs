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

    /**
     * Inserts a new blog post into SQL Server and saves its generated id on the model.
     * @param {BlogPost} blogPost - The new blog post model, with an id of zero before it is saved.
     * @returns {void} No return value, saves the blog post in the database.
     */
    public void Create(BlogPost blogPost)
    {
        _context.BlogPosts.Add(blogPost);
        _context.SaveChanges();
    }

    /**
     * Saves the changed fields of an existing blog post in SQL Server.
     * @param {BlogPost} blogPost - An existing blog post model with its id and updated fields.
     * @returns {void} No return value, saves the changes in the database.
     */
    public void Update(BlogPost blogPost)
    {
        _context.BlogPosts.Update(blogPost);
        _context.SaveChanges();
    }

    /**
     * Removes an existing blog post from SQL Server.
     * @param {BlogPost} blogPost - The existing blog post model to delete.
     * @returns {void} No return value, deletes the blog post from the database.
     */
    public void Delete(BlogPost blogPost)
    {
        _context.BlogPosts.Remove(blogPost);
        _context.SaveChanges();
    }
}
