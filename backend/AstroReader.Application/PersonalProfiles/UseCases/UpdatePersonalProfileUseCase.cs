using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;

namespace AstroReader.Application.PersonalProfiles.UseCases;

public class UpdatePersonalProfileUseCase : IUpdatePersonalProfileUseCase
{
    private readonly IPersonalProfileRepository _personalProfileRepository;

    public UpdatePersonalProfileUseCase(IPersonalProfileRepository personalProfileRepository)
    {
        _personalProfileRepository = personalProfileRepository;
    }

    public async Task<PersonalProfileDetailDto> ExecuteAsync(
        Guid id,
        UpdatePersonalProfileRequest request,
        Guid? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var personalProfile = await _personalProfileRepository.GetTrackedByIdAsync(id, ownerUserId, cancellationToken);

        if (personalProfile is null)
        {
            throw new KeyNotFoundException($"Personal profile '{id}' was not found.");
        }

        var (birthDate, birthTime) = PersonalProfileRequestValidation.ParseInputFormat(request);
        PersonalProfileRequestValidation.ValidateWriteRequest(request);

        personalProfile.Update(
            request.FullName,
            birthDate,
            birthTime,
            request.BirthPlace,
            (decimal)request.Latitude,
            (decimal)request.Longitude,
            (short)request.TimezoneOffsetMinutes,
            request.SelfPerceptionFocus,
            request.CurrentChallenge,
            request.DesiredInsight,
            request.SelfDescription);

        await _personalProfileRepository.SaveChangesAsync(cancellationToken);
        return PersonalProfileMappings.ToDetailDto(personalProfile);
    }
}
