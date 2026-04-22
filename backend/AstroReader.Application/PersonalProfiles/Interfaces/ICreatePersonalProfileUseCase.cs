using AstroReader.Application.PersonalProfiles.DTOs;

namespace AstroReader.Application.PersonalProfiles.Interfaces;

public interface ICreatePersonalProfileUseCase
{
    Task<PersonalProfileDetailDto> ExecuteAsync(
        CreatePersonalProfileRequest request,
        CancellationToken cancellationToken = default);
}
