using PortfolioApi.Models;
using PortfolioApi.Repositories;

namespace PortfolioApi.Services;

public class BlogPostService
{
    private readonly BlogPostRepository _blogPostRepository;

    /**
     * Receives the blog post repository through dependency injection.
     * @param {BlogPostRepository} blogPostRepository - Repository used to access blog post data.
     * @returns {void} No return value, initializes the service.
     */
    public BlogPostService(BlogPostRepository blogPostRepository)
    {
        _blogPostRepository = blogPostRepository;
    }

    /**
     * Retrieves all blog posts from the repository. No parameters are required.
     * @returns {BlogPost[]} The complete blog post list.
     */
    public BlogPost[] GetAll()
    {
        return _blogPostRepository.GetAll();
    }

    /**
     * Checks the requested id before retrieving a blog post from the repository.
     * @param {int} id - The blog post id, which must be greater than zero.
     * @returns {BlogPost?} The blog post, or null if the id is invalid or the blog post does not exist.
     */
    public BlogPost? GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return _blogPostRepository.GetById(id);
    }
}
