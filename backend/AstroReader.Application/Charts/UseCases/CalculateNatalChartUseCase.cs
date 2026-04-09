using System;
using System.Linq;
using System.Text.Json;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

using AstroReader.Application.Interpretations;

namespace AstroReader.Application.Charts.UseCases;

public class CalculateNatalChartUseCase : ICalculateNatalChartUseCase
{
    private readonly IAstroCalculationEngine _engine;
    private readonly IInterpretationEngine _interpretationEngine;

    public CalculateNatalChartUseCase(IAstroCalculationEngine engine, IInterpretationEngine interpretationEngine)
    {
        _engine = engine;
        _interpretationEngine = interpretationEngine;
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
            Sign = GetSignFromDegree(kvp.Value.AbsoluteDegree),
            SignDegree = kvp.Value.AbsoluteDegree % 30,
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
            Interpretation = _interpretationEngine.GenerateBaseInterpretation(natalChart)
        };
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
}
