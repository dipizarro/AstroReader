using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;

namespace AstroReader.Application.SavedCharts.UseCases;

public class GetSavedChartsUseCase : IGetSavedChartsUseCase
{
    private readonly ISavedChartRepository _savedChartRepository;

    public GetSavedChartsUseCase(ISavedChartRepository savedChartRepository)
    {
        _savedChartRepository = savedChartRepository;
    }

    public async Task<IReadOnlyList<SavedChartListItemDto>> ExecuteAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        return await _savedChartRepository.GetListItemsAsync(ownerUserId, cancellationToken);
    }
}
