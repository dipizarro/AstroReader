using AstroReader.Application.SavedCharts.DTOs;

namespace AstroReader.Application.SavedCharts.Interfaces;

public interface ISaveChartUseCase
{
    Task<SavedChartDetailDto> ExecuteAsync(SaveChartRequest request, CancellationToken cancellationToken = default);
}
