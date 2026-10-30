using PortfolioApi.Dtos.BlogPosts;

namespace PortfolioApi.Services;

public interface IBlogPostService
{
    Task<BlogPostDto[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BlogPostDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BlogPostDto> CreateAsync(CreateBlogPostDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateBlogPostDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
