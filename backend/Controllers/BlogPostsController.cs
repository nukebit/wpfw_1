using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.BlogPosts;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    /**
     * Receives the blog post service through dependency injection.
     * @param {IBlogPostService} blogPostService - Service used to retrieve blog posts.
     * @returns {void} No return value, initializes the controller.
     */
    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    /**
     * Requests all blog posts as output DTOs. No parameters are required.
     * @returns {Task<ActionResult<BlogPostDto[]>>} HTTP 200 with the blog post list as JSON.
     */
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BlogPostDto[]>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _blogPostService.GetAllAsync(cancellationToken));
    }

    /**
     * Requests one blog post from the service and chooses the HTTP response.
     * @param {int} id - The id of the requested blog post.
     * @returns {Task<ActionResult<BlogPostDto>>} HTTP 200 with the blog post DTO, or HTTP 404 with error details if it does not exist.
     */
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var blogPost = await _blogPostService.GetByIdAsync(id, cancellationToken);

        if (blogPost is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Blog post not found.",
                detail: $"No blog post exists with id {id}.");
        }

        return Ok(blogPost);
    }

    /**
     * Creates a blog post from the validated JSON request body.
     * @param {CreateBlogPostDto} dto - Required title, category, content, and publication date in YYYY-MM-DD format.
     * @returns {Task<ActionResult<BlogPostDto>>} HTTP 201 with the created blog post and its URL, or HTTP 400 for invalid input.
     */
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlogPostDto>> Create([FromBody] CreateBlogPostDto dto, CancellationToken cancellationToken)
    {
        var blogPost = await _blogPostService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = blogPost.Id }, blogPost);
    }

    /**
     * Replaces the editable fields of an existing blog post using a validated JSON request body.
     * @param {int} id - The id of the blog post to update, taken from the URL.
     * @param {UpdateBlogPostDto} dto - Required title, category, content, and publication date in YYYY-MM-DD format.
     * @returns {Task<IActionResult>} HTTP 204 on success, HTTP 400 for invalid input, or HTTP 404 with error details for a missing blog post.
     */
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogPostDto dto, CancellationToken cancellationToken)
    {
        var updated = await _blogPostService.UpdateAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Blog post not found.",
                detail: $"No blog post exists with id {id}.");
        }

        return NoContent();
    }

    /**
     * Deletes a blog post with the requested id.
     * @param {int} id - The id of the blog post to delete, taken from the URL.
     * @returns {Task<IActionResult>} HTTP 204 on success, or HTTP 404 with error details for a missing blog post.
     */
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _blogPostService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Blog post not found.",
                detail: $"No blog post exists with id {id}.");
        }

        return NoContent();
    }
}
