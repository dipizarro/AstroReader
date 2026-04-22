using AstroReader.Application.PersonalProfiles.DTOs;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface IUpdatePersonalProfileUseCase
{
    Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid id,
        UpdatePersonalProfileRequest request,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default);
}
