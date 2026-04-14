using System;
using System.Linq;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Application.Charts.UseCases;

public class CalculateNatalChartUseCase : ICalculateNatalChartUseCase
{
    private readonly IAstroCalculationEngine _engine;
    private readonly IPremiumInterpretationCatalogProvider _premiumInterpretationCatalogProvider;
    private readonly IInterpretationAnalyzer _interpretationAnalyzer;
    private readonly IInterpretationComposer _interpretationComposer;

    public CalculateNatalChartUseCase(
        IAstroCalculationEngine engine,
        IPremiumInterpretationCatalogProvider premiumInterpretationCatalogProvider,
        IInterpretationAnalyzer interpretationAnalyzer,
        IInterpretationComposer interpretationComposer)
    {
        _engine = engine;
        _premiumInterpretationCatalogProvider = premiumInterpretationCatalogProvider;
        _interpretationAnalyzer = interpretationAnalyzer;
        _interpretationComposer = interpretationComposer;
    }

    public CalculateChartResponse Execute(CalculateChartRequest request)
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

        // EXTRA: Obtener Ascendente para el Summary
        var ascendantSign = GetSignFromDegree(engineResult.AscendantDegree);
        var sunSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Sun)?.Sign ?? ZodiacSign.Aries;
        var moonSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Moon)?.Sign ?? ZodiacSign.Aries;

        var interpretation = BuildPremiumInterpretation(natalChart, sunSign, moonSign, ascendantSign);

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
        ZodiacSign ascendantSign)
    {
        var mercurySign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Mercury)?.Sign ?? ZodiacSign.Aries;
        var venusSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Venus)?.Sign ?? ZodiacSign.Aries;
        var marsSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Mars)?.Sign ?? ZodiacSign.Aries;

        var coverageAssessment = PremiumInterpretationCoverageEvaluator.Evaluate(
            _premiumInterpretationCatalogProvider.GetCatalog(),
            sunSign,
            moonSign,
            ascendantSign,
            mercurySign,
            venusSign,
            marsSign);

        if (!coverageAssessment.IsComplete)
        {
            var status = coverageAssessment.HasAnyCoverage
                ? InterpretationCoverageStatus.Partial
                : InterpretationCoverageStatus.Fallback;

            return PremiumInterpretationFallbackFactory.Create(
                sunSign,
                moonSign,
                ascendantSign,
                coverageAssessment.ToDto(status));
        }

        try
        {
            var analysis = _interpretationAnalyzer.Analyze(natalChart);
            var composition = _interpretationComposer.Compose(natalChart, analysis);
            return PremiumInterpretationResponseMapper.MapComposition(
                composition,
                coverageAssessment.ToDto(
                    InterpretationCoverageStatus.Complete,
                    PremiumInterpretationResponseMapper.PrimaryComposedBlocks));
        }
        catch (PremiumInterpretationCatalogException)
        {
            return PremiumInterpretationFallbackFactory.Create(
                sunSign,
                moonSign,
                ascendantSign,
                coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback));
        }
        catch (PremiumInterpretationAnalysisException)
        {
            return PremiumInterpretationFallbackFactory.Create(
                sunSign,
                moonSign,
                ascendantSign,
                coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback));
        }
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
