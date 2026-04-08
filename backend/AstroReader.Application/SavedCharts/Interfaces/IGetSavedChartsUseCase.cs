using AstroReader.Application.SavedCharts.DTOs;

namespace AstroReader.Application.SavedCharts.Interfaces;

public interface IGetSavedChartsUseCase
{
    Task<IReadOnlyList<SavedChartListItemDto>> ExecuteAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default);
}
