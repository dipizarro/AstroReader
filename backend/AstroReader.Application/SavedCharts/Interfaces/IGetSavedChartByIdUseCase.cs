using AstroReader.Application.SavedCharts.DTOs;

namespace AstroReader.Application.SavedCharts.Interfaces;

public interface IGetSavedChartByIdUseCase
{
    Task<SavedChartDetailDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
