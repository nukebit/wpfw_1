using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.BlogPosts;
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
     * Requests all blog posts as output DTOs. No parameters are required.
     * @returns {ActionResult<BlogPostDto[]>} HTTP 200 with the blog post list as JSON.
     */
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<BlogPostDto[]> GetAll()
    {
        return Ok(_blogPostService.GetAll());
    }

    /**
     * Requests one blog post from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested blog post.
     * @returns {ActionResult<BlogPostDto>} HTTP 200 with the blog post DTO as JSON, or HTTP 404 if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BlogPostDto> GetById(int id)
    {
        var blogPost = _blogPostService.GetById(id);

        if (blogPost is null)
        {
            return NotFound();
        }

        return Ok(blogPost);
    }

    /**
     * Creates a blog post from the validated JSON request body.
     * @param {CreateBlogPostDto} dto - Required title, category, content, and publication date in YYYY-MM-DD format.
     * @returns {ActionResult<BlogPostDto>} HTTP 201 with the created blog post and its URL, or HTTP 400 for invalid input.
     */
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<BlogPostDto> Create([FromBody] CreateBlogPostDto dto)
    {
        var blogPost = _blogPostService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = blogPost.Id }, blogPost);
    }

    /**
     * Replaces the editable fields of an existing blog post using a validated JSON request body.
     * @param {int} id - The id of the blog post to update, taken from the URL.
     * @param {UpdateBlogPostDto} dto - Required title, category, content, and publication date in YYYY-MM-DD format.
     * @returns {IActionResult} HTTP 204 on success, HTTP 400 for invalid input, or HTTP 404 if the blog post does not exist.
     */
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateBlogPostDto dto)
    {
        var updated = _blogPostService.Update(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /**
     * Deletes a blog post with the requested id.
     * @param {int} id - The id of the blog post to delete, taken from the URL.
     * @returns {IActionResult} HTTP 204 on success, or HTTP 404 if the blog post does not exist.
     */
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var deleted = _blogPostService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
