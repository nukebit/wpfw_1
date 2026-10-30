using PortfolioApi.Dtos.BlogPosts;
using PortfolioApi.Models;
using PortfolioApi.Repositories;

namespace PortfolioApi.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _blogPostRepository;

    /**
     * Receives the blog post repository through dependency injection.
     * @param {IBlogPostRepository} blogPostRepository - Repository used to access blog post data.
     * @returns {void} No return value, initializes the service.
     */
    public BlogPostService(IBlogPostRepository blogPostRepository)
    {
        _blogPostRepository = blogPostRepository;
    }

    /**
     * Retrieves all blog posts and converts them to output DTOs. No parameters are required.
     * @returns {Task<BlogPostDto[]>} The complete blog post list as DTOs.
     */
    public async Task<BlogPostDto[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _blogPostRepository.GetAllAsync(cancellationToken);
        return items.Select(ToDto).ToArray();
    }

    /**
     * Checks the requested id before retrieving a blog post from the repository.
     * @param {int} id - The blog post id, which must be greater than zero.
     * @returns {Task<BlogPostDto?>} The blog post DTO, or null if the id is invalid or the blog post does not exist.
     */
    public async Task<BlogPostDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        var blogPost = await _blogPostRepository.GetByIdAsync(id, cancellationToken);

        if (blogPost is null)
        {
            return null;
        }

        return ToDto(blogPost);
    }

    /**
     * Converts validated input to a blog post, trims its text, and saves it through the repository.
     * @param {CreateBlogPostDto} dto - Blog post input that has passed the controller's DTO validation.
     * @returns {Task<BlogPostDto>} The saved blog post with its database-generated id.
     */
    public async Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        var publishedOn = dto.PublishedOn
            ?? throw new ArgumentException("Publication date is required.", nameof(dto));

        var blogPost = new BlogPost
        {
            Title = dto.Title.Trim(),
            Category = dto.Category.Trim(),
            Content = dto.Content.Trim(),
            PublishedOn = publishedOn
        };

        await _blogPostRepository.CreateAsync(blogPost, cancellationToken);
        return ToDto(blogPost);
    }

    /**
     * Finds a blog post and replaces its editable fields with validated input.
     * @param {int} id - The blog post id, which must be greater than zero.
     * @param {UpdateBlogPostDto} dto - Blog post input that has passed the controller's DTO validation.
     * @returns {Task<bool>} True when the blog post is updated, or false if the id is invalid or the blog post does not exist.
     */
    public async Task<bool> UpdateAsync(int id, UpdateBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return false;
        }

        var blogPost = await _blogPostRepository.GetByIdAsync(id, cancellationToken);

        if (blogPost is null)
        {
            return false;
        }

        var publishedOn = dto.PublishedOn
            ?? throw new ArgumentException("Publication date is required.", nameof(dto));

        blogPost.Title = dto.Title.Trim();
        blogPost.Category = dto.Category.Trim();
        blogPost.Content = dto.Content.Trim();
        blogPost.PublishedOn = publishedOn;

        await _blogPostRepository.UpdateAsync(blogPost, cancellationToken);
        return true;
    }

    /**
     * Finds a blog post and asks the repository to delete it.
     * @param {int} id - The blog post id, which must be greater than zero.
     * @returns {Task<bool>} True when the blog post is deleted, or false if the id is invalid or the blog post does not exist.
     */
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return false;
        }

        var blogPost = await _blogPostRepository.GetByIdAsync(id, cancellationToken);

        if (blogPost is null)
        {
            return false;
        }

        await _blogPostRepository.DeleteAsync(blogPost, cancellationToken);
        return true;
    }

    /**
     * Copies the public blog post fields from a database model to an output DTO.
     * @param {BlogPost} blogPost - The blog post model retrieved from or saved to the database.
     * @returns {BlogPostDto} The blog post data returned to API clients.
     */
    private static BlogPostDto ToDto(BlogPost blogPost)
    {
        return new BlogPostDto
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Category = blogPost.Category,
            Content = blogPost.Content,
            PublishedOn = blogPost.PublishedOn
        };
    }
}
