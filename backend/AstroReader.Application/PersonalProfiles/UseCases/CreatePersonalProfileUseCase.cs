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
        var (birthDate, birthTime) = ParseInputFormat(request);
        ValidateRequest(request);

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

    private static (DateOnly BirthDate, TimeOnly BirthTime) ParseInputFormat(CreatePersonalProfileRequest request)
    {
        if (!DateOnly.TryParseExact(request.BirthDate, "yyyy-MM-dd", out var birthDate))
        {
            throw new ArgumentException("Formato de fecha inválido. Se requiere YYYY-MM-DD.");
        }

        if (!TimeOnly.TryParseExact(request.BirthTime, "HH:mm", out var birthTime))
        {
            throw new ArgumentException("Formato de hora inválido. Se requiere HH:mm.");
        }

        return (birthDate, birthTime);
    }

    private static void ValidateRequest(CreatePersonalProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin fullName.");
        }

        if (string.IsNullOrWhiteSpace(request.BirthPlace))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin birthPlace.");
        }

        if (string.IsNullOrWhiteSpace(request.SelfPerceptionFocus))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin selfPerceptionFocus.");
        }

        if (string.IsNullOrWhiteSpace(request.CurrentChallenge))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin currentChallenge.");
        }

        if (string.IsNullOrWhiteSpace(request.DesiredInsight))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin desiredInsight.");
        }

        if (double.IsNaN(request.Latitude) || double.IsInfinity(request.Latitude))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una latitud inválida.");
        }

        if (double.IsNaN(request.Longitude) || double.IsInfinity(request.Longitude))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una longitud inválida.");
        }

        if (request.Latitude is < -90 or > 90)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una latitud fuera de rango.");
        }

        if (request.Longitude is < -180 or > 180)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una longitud fuera de rango.");
        }

        if (request.TimezoneOffsetMinutes is < -720 or > 840)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con un offset horario fuera de rango.");
        }
    }
}
