using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;

namespace AstroReader.Application.SavedCharts.UseCases;

public class GetSavedChartByIdUseCase : IGetSavedChartByIdUseCase
{
    private readonly ISavedChartRepository _savedChartRepository;

    public GetSavedChartByIdUseCase(ISavedChartRepository savedChartRepository)
    {
        _savedChartRepository = savedChartRepository;
    }

    public async Task<SavedChartDetailDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var savedChart = await _savedChartRepository.GetByIdAsync(id, cancellationToken);

        if (savedChart is null)
        {
            throw new KeyNotFoundException($"Saved chart '{id}' was not found.");
        }

        return SavedChartMappings.ToDetailDto(savedChart);
    }
}
