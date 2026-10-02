using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogPostsController : ControllerBase
{
    private readonly BlogPostService _blogPostService;

    /**
     * Receives the blog post service through dependency injection.
     * @param {BlogPostService} blogPostService - Service used to retrieve blog posts.
     * @returns {void} No return value, initializes the controller.
     */
    public BlogPostsController(BlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    /**
     * Requests all blog posts from the service. No parameters are required.
     * @returns {ActionResult<BlogPost[]>} HTTP 200 with the blog post list as JSON.
     */
    [HttpGet]
    public ActionResult<BlogPost[]> GetAll()
    {
        return Ok(_blogPostService.GetAll());
    }

    /**
     * Requests one blog post from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested blog post.
     * @returns {ActionResult<BlogPost>} HTTP 200 with the blog post as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BlogPost> GetById(int id)
    {
        var blogPost = _blogPostService.GetById(id);

        if (blogPost is null)
        {
            return NotFound();
        }

        return Ok(blogPost);
    }
}
