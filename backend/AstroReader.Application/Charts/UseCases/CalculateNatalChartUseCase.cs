using System;
using System.Linq;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Application.Charts.UseCases;

public class CalculateNatalChartUseCase : ICalculateNatalChartUseCase
{
    private sealed class NullPersonalProfileRepository : IPersonalProfileRepository
    {
        public Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("This repository only supports read operations returning no profile context.");
        }

        public Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(null);
        }

        public Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(null);
        }

        public Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(null);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private static readonly IPersonalProfileRepository EmptyPersonalProfileRepository = new NullPersonalProfileRepository();
    private readonly IAstroCalculationEngine _engine;
    private readonly IPersonalProfileRepository _personalProfileRepository;
    private readonly IPremiumInterpretationContextResolver _premiumInterpretationContextResolver;
    private readonly IInterpretationAnalyzer _interpretationAnalyzer;
    private readonly IInterpretationComposer _interpretationComposer;

    public CalculateNatalChartUseCase(
        IAstroCalculationEngine engine,
        IPersonalProfileRepository personalProfileRepository,
        IPremiumInterpretationContextResolver premiumInterpretationContextResolver,
        IInterpretationAnalyzer interpretationAnalyzer,
        IInterpretationComposer interpretationComposer)
    {
        _engine = engine;
        _personalProfileRepository = personalProfileRepository;
        _premiumInterpretationContextResolver = premiumInterpretationContextResolver;
        _interpretationAnalyzer = interpretationAnalyzer;
        _interpretationComposer = interpretationComposer;
    }

    public CalculateNatalChartUseCase(
        IAstroCalculationEngine engine,
        IPremiumInterpretationContextResolver premiumInterpretationContextResolver,
        IInterpretationAnalyzer interpretationAnalyzer,
        IInterpretationComposer interpretationComposer)
        : this(
            engine,
            EmptyPersonalProfileRepository,
            premiumInterpretationContextResolver,
            interpretationAnalyzer,
            interpretationComposer)
    {
    }

    public CalculateChartResponse Execute(CalculateChartRequest request)
    {
        return ExecuteAsync(request).GetAwaiter().GetResult();
    }

    public async Task<CalculateChartResponse> ExecuteAsync(CalculateChartRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Parsear fecha y hora usando tipos seguros nativos de .NET 8
        if (!DateOnly.TryParseExact(request.BirthDate, "yyyy-MM-dd", out var dateOnly))
        {
            throw new ArgumentException("Formato de fecha inválido. Se requiere YYYY-MM-DD.");
        }

        if (!TimeOnly.TryParseExact(request.BirthTime, "HH:mm", out var timeOnly))
        {
            throw new ArgumentException("Formato de hora inválido. Se requiere HH:mm.");
        }

        // 2. Combinar en DateTimeOffset usando el offset específico provisto (en minutos)
        var offsetSpan = TimeSpan.FromMinutes(request.TimezoneOffsetMinutes);
        var dateTimeOffset = new DateTimeOffset(
            dateOnly.Year, dateOnly.Month, dateOnly.Day,
            timeOnly.Hour, timeOnly.Minute, 0,
            offsetSpan
        );

        // 3. Extraer el instante exacto en UTC para la trazabilidad y el AstroEngine
        var utcDate = dateTimeOffset.UtcDateTime;

        // 4. Ejecutar el Engine Matemático con Coordenadas Reales
        var engineRequest = new AstroCalculationRequest(utcDate, request.Latitude, request.Longitude);
        var engineResult = _engine.Calculate(engineRequest);

        // 3. Mapear EngineResult (Data cruda) -> Entidades de Dominio
        var planets = engineResult.PlanetaryPositions.Select(kvp => new AstroReader.Domain.Entities.PlanetPosition
        {
            Planet = MapPlanetIdToEnum(kvp.Key),
            AbsoluteDegree = kvp.Value.AbsoluteDegree,
            Sign = MapSignIndexToEnum(kvp.Value.ZodiacSignIndex),
            SignDegree = kvp.Value.SignDegree,
            IsRetrograde = kvp.Value.IsRetrograde
        }).ToList();

        var houses = engineResult.Houses.Select(kvp => new AstroReader.Domain.Entities.HousePosition
        {
            HouseNumber = kvp.Key,
            AbsoluteDegree = kvp.Value,
            Sign = GetSignFromDegree(kvp.Value)
        }).ToList();

        var birthData = new BirthData(utcDate, request.TimezoneOffsetMinutes.ToString());
        var geoLoc = new GeoLocation(request.Latitude, request.Longitude);
        
        // Creamos la Carta Natal del Dominio (Pura e inmutable)
        var natalChart = new NatalChart(birthData, geoLoc, planets, houses);
        var readerProfile = await ResolveReaderProfileContextAsync(
            request.PersonalProfileId,
            dateOnly,
            timeOnly,
            request.Latitude,
            request.Longitude,
            request.TimezoneOffsetMinutes,
            cancellationToken);

        // EXTRA: Obtener Ascendente para el Summary
        var ascendantSign = GetSignFromDegree(engineResult.AscendantDegree);
        var sunSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Sun)?.Sign ?? ZodiacSign.Aries;
        var moonSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Moon)?.Sign ?? ZodiacSign.Aries;

        var interpretation = BuildPremiumInterpretation(natalChart, sunSign, moonSign, ascendantSign, readerProfile);

        // 4. Mapear Entidades de Dominio -> API Response DTO
        return new CalculateChartResponse
        {
            Metadata = new ChartMetadata
            {
                CalculatedForUtc = utcDate,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PlaceName = request.PlaceName
            },
            Summary = new AstroReader.Application.Charts.DTOs.ChartSummary
            {
                Sun = sunSign.ToString(),
                Moon = moonSign.ToString(),
                Ascendant = ascendantSign.ToString()
            },
            Planets = natalChart.Planets.Select(p => new AstroReader.Application.Charts.DTOs.PlanetPositionDto
            {
                Name = p.Planet.ToString(),
                Sign = p.Sign.ToString(),
                SignDegree = Math.Round(p.SignDegree, 2),
                AbsoluteDegree = Math.Round(p.AbsoluteDegree, 2),
                IsRetrograde = p.IsRetrograde
            }).ToList(),
            Houses = natalChart.Houses.Select(h => new AstroReader.Application.Charts.DTOs.HousePositionDto
            {
                Number = h.HouseNumber,
                Sign = h.Sign.ToString(),
                AbsoluteDegree = Math.Round(h.AbsoluteDegree, 2)
            }).ToList(),
            Interpretation = interpretation
        };
    }

    private ChartInterpretation BuildPremiumInterpretation(
        NatalChart natalChart,
        ZodiacSign sunSign,
        ZodiacSign moonSign,
        ZodiacSign ascendantSign,
        PremiumReaderProfileContext? readerProfile)
    {
        var context = _premiumInterpretationContextResolver.Resolve(natalChart, readerProfile);
        var coverageAssessment = context.Coverage;

        try
        {
            var analysis = _interpretationAnalyzer.Analyze(context);
            var composition = _interpretationComposer.Compose(context, analysis);
            var composedBlocks = PremiumInterpretationCompositionEvaluator.GetComposedBlocks(composition);
            var status = PremiumInterpretationCompositionEvaluator.DetermineStatus(context, composedBlocks);

            if (status == InterpretationCoverageStatus.Fallback)
            {
                return PremiumInterpretationFallbackFactory.Create(
                    sunSign,
                    moonSign,
                    ascendantSign,
                    coverageAssessment.ToDto(status),
                    readerProfile);
            }

            return PremiumInterpretationResponseMapper.MapComposition(
                composition,
                coverageAssessment.ToDto(
                    status,
                    composedBlocks));
        }
        catch (PremiumInterpretationCatalogException)
        {
            return PremiumInterpretationFallbackFactory.Create(
                sunSign,
                moonSign,
                ascendantSign,
                coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback),
                readerProfile);
        }
        catch (PremiumInterpretationAnalysisException)
        {
            return PremiumInterpretationFallbackFactory.Create(
                sunSign,
                moonSign,
                ascendantSign,
                coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback),
                readerProfile);
        }
    }

    private async Task<PremiumReaderProfileContext?> ResolveReaderProfileContextAsync(
        Guid? personalProfileId,
        DateOnly birthDate,
        TimeOnly birthTime,
        double latitude,
        double longitude,
        int timezoneOffsetMinutes,
        CancellationToken cancellationToken)
    {
        if (!personalProfileId.HasValue)
        {
            return null;
        }

        var personalProfile = await _personalProfileRepository.GetByIdAsync(
            personalProfileId.Value,
            ownerUserId: null,
            cancellationToken);

        if (personalProfile is null)
        {
            throw new KeyNotFoundException($"Personal profile '{personalProfileId.Value}' was not found.");
        }

        ValidatePersonalProfileMatchesRequest(
            personalProfile,
            birthDate,
            birthTime,
            latitude,
            longitude,
            timezoneOffsetMinutes);

        return PremiumReaderProfileContext.FromPersonalProfile(personalProfile);
    }

    private static void ValidatePersonalProfileMatchesRequest(
        PersonalProfile personalProfile,
        DateOnly birthDate,
        TimeOnly birthTime,
        double latitude,
        double longitude,
        int timezoneOffsetMinutes)
    {
        var mismatchedFields = new List<string>();

        if (personalProfile.BirthDate != birthDate)
        {
            mismatchedFields.Add("birthDate");
        }

        if (personalProfile.BirthTime != birthTime)
        {
            mismatchedFields.Add("birthTime");
        }

        if (!CoordinateMatches(personalProfile.Latitude, latitude))
        {
            mismatchedFields.Add("latitude");
        }

        if (!CoordinateMatches(personalProfile.Longitude, longitude))
        {
            mismatchedFields.Add("longitude");
        }

        if (personalProfile.TimezoneOffsetMinutes != timezoneOffsetMinutes)
        {
            mismatchedFields.Add("timezoneOffsetMinutes");
        }

        if (mismatchedFields.Count == 0)
        {
            return;
        }

        throw new PersonalProfileIntegrityException(
            "El personalProfileId enviado no coincide con los datos natales usados para calcular la carta. " +
            $"Campos inconsistentes: {string.Join(", ", mismatchedFields)}.");
    }

    private static bool CoordinateMatches(decimal profileCoordinate, double requestCoordinate)
    {
        var normalizedProfile = decimal.Round(profileCoordinate, 6, MidpointRounding.AwayFromZero);
        var normalizedRequest = decimal.Round((decimal)requestCoordinate, 6, MidpointRounding.AwayFromZero);
        return normalizedProfile == normalizedRequest;
    }

    private Planet MapPlanetIdToEnum(int id)
    {
        // Simple mock mapping: 0=Sun, 1=Moon (como lo definimos en el MockEngine)
        return id switch
        {
            0 => Planet.Sun,
            1 => Planet.Moon,
            2 => Planet.Mercury,
            3 => Planet.Venus,
            4 => Planet.Mars,
            5 => Planet.Jupiter,
            6 => Planet.Saturn,
            _ => Planet.Pluto
        };
    }

    private ZodiacSign GetSignFromDegree(double degree)
    {
        degree = (degree % 360 + 360) % 360; // Normalize
        int signIndex = (int)(degree / 30);
        return (ZodiacSign)signIndex;
    }

    private ZodiacSign MapSignIndexToEnum(int signIndex)
    {
        var normalizedIndex = ((signIndex % 12) + 12) % 12;
        return (ZodiacSign)normalizedIndex;
    }
}
