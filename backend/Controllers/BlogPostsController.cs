using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogPostsController : ControllerBase
{
    // temp data
    private static readonly BlogPost[] BlogPosts =
    {
        new BlogPost
        {
            Id = 1,
            Title = "Validate iedere API request",
            Category = "Laravel",
            Content = "Form requests houden validation rules buiten de controllers. Ongeldige data komt hierdoor niet in de application logic terecht.",
            PublishedOn = new DateOnly(2026, 9, 7)
        },
        new BlogPost
        {
            Id = 2,
            Title = "Bewaar state bij de juiste component",
            Category = "React",
            Content = "Local state is geschikt voor tijdelijke data. Gedeelde server data gaat via een duidelijke API layer.",
            PublishedOn = new DateOnly(2026, 9, 8)
        },
        new BlogPost
        {
            Id = 3,
            Title = "Maak API responses voorspelbaar",
            Category = "REST API",
            Content = "Een vaste JSON structure maakt succesvolle requests en errors eenvoudiger af te handelen in React.",
            PublishedOn = new DateOnly(2026, 9, 9)
        }
    };

    /**
     * Returns all blog posts from the temporary data. No parameters are required.
     * @returns {ActionResult<BlogPost[]>} HTTP 200 with the blog post list as JSON.
     */
    [HttpGet]
    public ActionResult<BlogPost[]> GetAll()
    {
        return Ok(BlogPosts);
    }

    /**
     * Looks up one blog post in the temporary data using its id.
     * @param {int} id - The id of the requested blog post.
     * @returns {ActionResult<BlogPost>} HTTP 200 with the blog post as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BlogPost> GetById(int id)
    {
        var blogPost = Array.Find(BlogPosts, blogPost => blogPost.Id == id);

        if (blogPost is null)
        {
            return NotFound();
        }

        return Ok(blogPost);
    }
}
