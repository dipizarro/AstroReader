using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;

namespace AstroReader.Application.PersonalProfiles.UseCases;

public class GetPersonalProfileBySavedChartIdUseCase : IGetPersonalProfileBySavedChartIdUseCase
{
    private readonly IPersonalProfileRepository _personalProfileRepository;

    public GetPersonalProfileBySavedChartIdUseCase(IPersonalProfileRepository personalProfileRepository)
    {
        _personalProfileRepository = personalProfileRepository;
    }

    public async Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid savedChartId,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var personalProfile = await _personalProfileRepository.GetBySavedChartIdAsync(
            savedChartId,
            ownerUserId,
            cancellationToken);

        if (personalProfile is null)
        {
            throw new KeyNotFoundException(
                $"Personal profile linked to saved chart '{savedChartId}' was not found.");
        }

        return PersonalProfileMappings.ToDetailDto(personalProfile);
    }
}
