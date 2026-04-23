using AstroReader.Application.PersonalProfiles.DTOs;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface IGetPersonalProfilesUseCase
{
    Task<IReadOnlyList<PersonalProfileListItemDto>> ExecuteAsync(
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default);
}
