using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.PersonalProfiles.UseCases;

public class CreatePersonalProfileUseCase : ICreatePersonalProfileUseCase
{
    private readonly IPersonalProfileRepository _personalProfileRepository;
    private readonly ISavedChartRepository _savedChartRepository;

    public CreatePersonalProfileUseCase(
        IPersonalProfileRepository personalProfileRepository,
        ISavedChartRepository savedChartRepository)
    {
        _personalProfileRepository = personalProfileRepository;
        _savedChartRepository = savedChartRepository;
    }

    public async Task<PersonalProfileDetailDto> ExecuteAsync(
        CreatePersonalProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var (birthDate, birthTime) = PersonalProfileRequestValidation.ParseInputFormat(request);
        PersonalProfileRequestValidation.ValidateWriteRequest(request);

        if (request.SavedChartId.HasValue)
        {
            var savedChart = await _savedChartRepository.GetByIdAsync(
                request.SavedChartId.Value,
                null,
                cancellationToken);

            if (savedChart is null)
            {
                throw new PersonalProfileIntegrityException(
                    "No se puede vincular el perfil a una carta guardada inexistente.");
            }

            if (request.UserId.HasValue &&
                savedChart.UserId.HasValue &&
                savedChart.UserId.Value != request.UserId.Value)
            {
                throw new PersonalProfileIntegrityException(
                    "La carta guardada no pertenece al mismo usuario del perfil.");
            }

            var existingProfile = await _personalProfileRepository.GetBySavedChartIdAsync(
                request.SavedChartId.Value,
                null,
                cancellationToken);

            if (existingProfile is not null)
            {
                throw new PersonalProfileIntegrityException(
                    "La carta guardada ya tiene un perfil personal asociado.");
            }
        }

        var personalProfile = new PersonalProfile(
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
            request.SelfDescription,
            request.UserId,
            request.SavedChartId);

        var persistedProfile = await _personalProfileRepository.AddAsync(personalProfile, cancellationToken);
        return PersonalProfileMappings.ToDetailDto(persistedProfile);
    }
}
