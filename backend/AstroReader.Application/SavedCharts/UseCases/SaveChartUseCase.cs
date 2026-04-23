using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Exceptions;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.SavedCharts.UseCases;

public class SaveChartUseCase : ISaveChartUseCase
{
    private readonly ICalculateNatalChartUseCase _calculateNatalChartUseCase;
    private readonly IPersonalProfileRepository _personalProfileRepository;
    private readonly IAstroEngineTechnicalMetadataProvider _astroEngineTechnicalMetadataProvider;
    private readonly ISavedChartRepository _savedChartRepository;
    private readonly ISavedChartTransactionManager _transactionManager;

    public SaveChartUseCase(
        ICalculateNatalChartUseCase calculateNatalChartUseCase,
        IPersonalProfileRepository personalProfileRepository,
        IAstroEngineTechnicalMetadataProvider astroEngineTechnicalMetadataProvider,
        ISavedChartRepository savedChartRepository,
        ISavedChartTransactionManager transactionManager)
    {
        _calculateNatalChartUseCase = calculateNatalChartUseCase;
        _personalProfileRepository = personalProfileRepository;
        _astroEngineTechnicalMetadataProvider = astroEngineTechnicalMetadataProvider;
        _savedChartRepository = savedChartRepository;
        _transactionManager = transactionManager;
    }

    public async Task<SavedChartDetailDto> ExecuteAsync(
        SaveChartRequest request,
        CancellationToken cancellationToken = default)
    {
        var (birthDate, birthTime) = ParseInputFormat(request);
        ValidateSaveIntegrity(request);
        var calculatedChart = await CalculateChartOrThrowIntegrityErrorAsync(request, cancellationToken);
        ValidateCalculatedChartIntegrity(calculatedChart, request);

        return await _transactionManager.ExecuteAsync(async transactionCancellationToken =>
        {
            var personalProfile = await ResolveTrackedPersonalProfileAsync(request, transactionCancellationToken);
            var birthInstantUtc = BuildBirthInstantUtc(birthDate, birthTime, request.TimezoneOffsetMinutes);
            var snapshotJson = SavedChartMappings.SerializeSnapshot(calculatedChart);
            var technicalMetadata = _astroEngineTechnicalMetadataProvider.GetCurrent();

            var savedChart = new SavedChart(
                request.ProfileName,
                request.PlaceName,
                request.TimezoneIana,
                birthDate,
                birthTime,
                (short)request.TimezoneOffsetMinutes,
                birthInstantUtc,
                (decimal)request.Latitude,
                (decimal)request.Longitude,
                calculatedChart.Summary.Sun,
                calculatedChart.Summary.Moon,
                calculatedChart.Summary.Ascendant,
                technicalMetadata.CalculationEngine,
                technicalMetadata.HouseSystemCode,
                SavedChart.CurrentSnapshotVersion,
                snapshotJson,
                request.UserId);

            var persistedChart = await _savedChartRepository.AddAsync(savedChart, transactionCancellationToken);

            if (personalProfile is not null)
            {
                personalProfile.LinkToSavedChart(persistedChart.Id);
                await _personalProfileRepository.SaveChangesAsync(transactionCancellationToken);
            }

            return SavedChartMappings.ToDetailDto(persistedChart, personalProfile);
        }, cancellationToken);
    }

    private static (DateOnly BirthDate, TimeOnly BirthTime) ParseInputFormat(SaveChartRequest request)
    {
        // Format validation: checks lexical correctness of the incoming birth fields.
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

    private static void ValidateSaveIntegrity(SaveChartRequest request)
    {
        // Integrity validation: protects persisted records from incomplete or semantically invalid data.
        if (string.IsNullOrWhiteSpace(request.ProfileName))
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar sin un nombre visible.");
        }

        if (double.IsNaN(request.Latitude) || double.IsInfinity(request.Latitude))
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar con una latitud inválida.");
        }

        if (double.IsNaN(request.Longitude) || double.IsInfinity(request.Longitude))
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar con una longitud inválida.");
        }

        if (request.Latitude is < -90 or > 90)
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar con una latitud fuera de rango.");
        }

        if (request.Longitude is < -180 or > 180)
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar con una longitud fuera de rango.");
        }

        if (request.TimezoneOffsetMinutes is < -720 or > 840)
        {
            throw new SavedChartIntegrityException("La carta no se puede guardar con un offset horario fuera de rango.");
        }
    }

    private async Task<CalculateChartResponse> CalculateChartOrThrowIntegrityErrorAsync(
        SaveChartRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _calculateNatalChartUseCase.ExecuteAsync(new CalculateChartRequest
            {
                BirthDate = request.BirthDate,
                BirthTime = request.BirthTime,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TimezoneOffsetMinutes = request.TimezoneOffsetMinutes,
                PlaceName = request.PlaceName,
                PersonalProfileId = request.PersonalProfileId
            }, cancellationToken);
        }
        catch (PersonalProfileIntegrityException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (SavedChartIntegrityException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new SavedChartIntegrityException(
                "No fue posible generar una carta válida para guardar con los datos natales entregados.");
        }
    }

    private async Task<PersonalProfile?> ResolveTrackedPersonalProfileAsync(
        SaveChartRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.PersonalProfileId.HasValue)
        {
            return null;
        }

        var personalProfile = await _personalProfileRepository.GetTrackedByIdAsync(
            request.PersonalProfileId.Value,
            ownerUserId: null,
            cancellationToken);

        if (personalProfile is null)
        {
            throw new KeyNotFoundException(
                $"Personal profile '{request.PersonalProfileId.Value}' was not found.");
        }

        if (request.UserId.HasValue &&
            personalProfile.UserId.HasValue &&
            personalProfile.UserId.Value != request.UserId.Value)
        {
            throw new PersonalProfileIntegrityException(
                "El perfil personal no pertenece al mismo usuario de la carta que se intenta guardar.");
        }

        if (personalProfile.SavedChartId.HasValue)
        {
            throw new PersonalProfileIntegrityException(
                "El perfil personal ya está vinculado a otra carta guardada.");
        }

        return personalProfile;
    }

    private static void ValidateCalculatedChartIntegrity(CalculateChartResponse chart, SaveChartRequest request)
    {
        if (chart.Summary is null)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene un resumen válido para ser guardada.");
        }

        if (string.IsNullOrWhiteSpace(chart.Summary.Sun) ||
            string.IsNullOrWhiteSpace(chart.Summary.Moon) ||
            string.IsNullOrWhiteSpace(chart.Summary.Ascendant))
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene la tríada central completa.");
        }

        if (chart.Planets is null || chart.Planets.Count == 0)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene posiciones planetarias para guardarse.");
        }

        var hasSun = chart.Planets.Any(x => string.Equals(x.Name, "Sun", StringComparison.OrdinalIgnoreCase));
        var hasMoon = chart.Planets.Any(x => string.Equals(x.Name, "Moon", StringComparison.OrdinalIgnoreCase));

        if (!hasSun || !hasMoon)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene los puntos planetarios mínimos para guardarse.");
        }

        if (chart.Houses is null || chart.Houses.Count == 0)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene casas para guardarse.");
        }

        var hasFirstHouse = chart.Houses.Any(x => x.Number == 1);

        if (!hasFirstHouse)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene la Casa 1 necesaria para guardarse.");
        }

        if (chart.Metadata is null)
        {
            throw new SavedChartIntegrityException("La carta calculada no contiene metadata suficiente para guardarse.");
        }

        if (Math.Abs(chart.Metadata.Latitude - request.Latitude) > 0.000001 ||
            Math.Abs(chart.Metadata.Longitude - request.Longitude) > 0.000001)
        {
            throw new SavedChartIntegrityException("La carta calculada no coincide con las coordenadas solicitadas.");
        }
    }

    private static DateTime BuildBirthInstantUtc(DateOnly birthDate, TimeOnly birthTime, int timezoneOffsetMinutes)
    {
        var dateTimeOffset = new DateTimeOffset(
            birthDate.Year,
            birthDate.Month,
            birthDate.Day,
            birthTime.Hour,
            birthTime.Minute,
            0,
            TimeSpan.FromMinutes(timezoneOffsetMinutes));

        return dateTimeOffset.UtcDateTime;
    }
}
