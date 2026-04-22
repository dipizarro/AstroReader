using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;

namespace AstroReader.Application.PersonalProfiles.UseCases;

public class GetPersonalProfileByIdUseCase : IGetPersonalProfileByIdUseCase
{
    private readonly IPersonalProfileRepository _personalProfileRepository;

    public GetPersonalProfileByIdUseCase(IPersonalProfileRepository personalProfileRepository)
    {
        _personalProfileRepository = personalProfileRepository;
    }

    public async Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid id,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var personalProfile = await _personalProfileRepository.GetByIdAsync(id, ownerUserId, cancellationToken);

        if (personalProfile is null)
        {
            throw new KeyNotFoundException($"Personal profile '{id}' was not found.");
        }

        return PersonalProfileMappings.ToDetailDto(personalProfile);
    }
}
