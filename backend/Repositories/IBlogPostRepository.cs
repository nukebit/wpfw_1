using PortfolioApi.Models;

namespace PortfolioApi.Repositories;

public interface IBlogPostRepository
{
    Task<IReadOnlyList<BlogPost>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(BlogPost blogPost, CancellationToken cancellationToken = default);
    Task UpdateAsync(BlogPost blogPost, CancellationToken cancellationToken = default);
    Task DeleteAsync(BlogPost blogPost, CancellationToken cancellationToken = default);
}
