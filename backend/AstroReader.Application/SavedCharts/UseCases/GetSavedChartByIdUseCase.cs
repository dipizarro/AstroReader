using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.SavedCharts.Interfaces;

namespace AstroReader.Application.SavedCharts.UseCases;

public class GetSavedChartByIdUseCase : IGetSavedChartByIdUseCase
{
    private readonly ISavedChartRepository _savedChartRepository;
    private readonly IPersonalProfileRepository _personalProfileRepository;

    public GetSavedChartByIdUseCase(
        ISavedChartRepository savedChartRepository,
        IPersonalProfileRepository personalProfileRepository)
    {
        _savedChartRepository = savedChartRepository;
        _personalProfileRepository = personalProfileRepository;
    }

    public async Task<SavedChartDetailDto> ExecuteAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
    {
        var savedChart = await _savedChartRepository.GetByIdAsync(id, ownerUserId, cancellationToken);

        if (savedChart is null)
        {
            throw new KeyNotFoundException($"Saved chart '{id}' was not found.");
        }

        var personalProfile = await _personalProfileRepository.GetBySavedChartIdAsync(
            savedChart.Id,
            ownerUserId,
            cancellationToken);

        return SavedChartMappings.ToDetailDto(savedChart, personalProfile);
    }
}
