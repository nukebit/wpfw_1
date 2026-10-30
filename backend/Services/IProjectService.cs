using PortfolioApi.Dtos.Projects;

namespace PortfolioApi.Services;

public interface IProjectService
{
    Task<ProjectDto[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
