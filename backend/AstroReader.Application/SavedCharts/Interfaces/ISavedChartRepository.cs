using AstroReader.Domain.Entities;

namespace AstroReader.Application.SavedCharts.Interfaces;

public interface ISavedChartRepository
{
    Task<SavedChart> AddAsync(SavedChart savedChart, CancellationToken cancellationToken = default);
    Task<SavedChart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SavedChart>> GetAllAsync(CancellationToken cancellationToken = default);
}
