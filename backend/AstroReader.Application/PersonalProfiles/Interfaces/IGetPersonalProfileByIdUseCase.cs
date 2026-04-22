using AstroReader.Application.PersonalProfiles.DTOs;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface IGetPersonalProfileByIdUseCase
{
    Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid id,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default);
}
