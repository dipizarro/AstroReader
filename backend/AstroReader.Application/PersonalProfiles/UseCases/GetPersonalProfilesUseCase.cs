using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;

namespace AstroReader.Application.PersonalProfiles.UseCases;

public class GetPersonalProfilesUseCase : IGetPersonalProfilesUseCase
{
    private readonly IPersonalProfileRepository _personalProfileRepository;

    public GetPersonalProfilesUseCase(IPersonalProfileRepository personalProfileRepository)
    {
        _personalProfileRepository = personalProfileRepository;
    }

    public async Task<IReadOnlyList<PersonalProfileListItemDto>> ExecuteAsync(
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var profiles = await _personalProfileRepository.GetListAsync(ownerUserId, cancellationToken);

        return profiles
            .Select(PersonalProfileMappings.ToListItemDto)
            .ToList();
    }
}
