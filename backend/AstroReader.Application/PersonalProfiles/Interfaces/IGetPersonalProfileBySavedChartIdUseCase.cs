using AstroReader.Application.PersonalProfiles.DTOs;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface IGetPersonalProfileBySavedChartIdUseCase
{
    Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid savedChartId,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default);
}
