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
        // 1. Parsear input local
        if (!DateTime.TryParse($"{request.BirthDate}T{request.BirthTime}:00", out var localDate))
        {
            throw new ArgumentException("Formato de fecha/hora inválido para cálculo trigonométrico.");
        }

        // Restamos el offset para derivar la hora UTC exacta
        // (Ej: Si es Buenos Aires, GMT-3 -> Offset es -180. 14:30 - (-180 min) = 17:30 UTC)
        var utcDate = localDate.AddMinutes(-request.TimezoneOffsetMinutes);

        // 2. Ejecutar el Engine Matemático con Coordenadas Reales
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
                Longitude = request.Longitude
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
