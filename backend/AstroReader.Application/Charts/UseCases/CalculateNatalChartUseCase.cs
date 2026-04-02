using System;
using System.Linq;
using System.Text.Json;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Application.Charts.UseCases;

public class CalculateNatalChartUseCase : ICalculateNatalChartUseCase
{
    private readonly IAstroCalculationEngine _engine;

    public CalculateNatalChartUseCase(IAstroCalculationEngine engine)
    {
        _engine = engine;
    }

    public CalculateChartResponse Execute(CalculateChartRequest request)
    {
        // 1. Validar y parsear input básico (En una V2 será UTC que manda el front)
        if (!DateTime.TryParse($"{request.BirthDate}T{request.BirthTime}:00", out var parsedDate))
        {
            throw new ArgumentException("Formato de fecha/hora inválido para cálculo trigonométrico.");
        }

        // Mockeamos coordenadas porque aún la API recibe el place como string ("Buenos Aires")
        // En un futuro el DTO recibirá Latitude y Longitude directamente
        var mockLatitude = -34.6;
        var mockLongitude = -58.4;

        // 2. Ejecutar el Engine Matemático
        var engineRequest = new AstroCalculationRequest(parsedDate.ToUniversalTime(), mockLatitude, mockLongitude);
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

        var birthData = new BirthData(parsedDate.ToUniversalTime(), "Unknown");
        var geoLoc = new GeoLocation(mockLatitude, mockLongitude);
        
        // Creamos la Carta Natal del Dominio (Pura e inmutable)
        var natalChart = new NatalChart(birthData, geoLoc, planets, houses);

        // EXTRA: Obtener Ascendente para el Summary
        var ascendantSign = GetSignFromDegree(engineResult.AscendantDegree);
        var sunSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Sun)?.Sign ?? ZodiacSign.Aries;
        var moonSign = natalChart.Planets.FirstOrDefault(p => p.Planet == Planet.Moon)?.Sign ?? ZodiacSign.Aries;

        // 4. Mapear Entidades de Dominio -> API Response DTO
        return new CalculateChartResponse
        {
            Summary = new AstroReader.Application.Charts.DTOs.ChartSummary
            {
                Sun = sunSign.ToString(),
                Moon = moonSign.ToString(),
                Ascendant = ascendantSign.ToString()
            },
            Planets = natalChart.Planets.Select(p => new AstroReader.Application.Charts.DTOs.PlanetPosition
            {
                Name = p.Planet.ToString(),
                Sign = p.Sign.ToString(),
                Degree = Math.Round(p.SignDegree, 2)
            }).ToList(),
            Houses = natalChart.Houses.Select(h => new AstroReader.Application.Charts.DTOs.HousePosition
            {
                House = h.HouseNumber,
                Sign = h.Sign.ToString()
            }).ToList(),
            Interpretation = new ChartInterpretation
            {
                Headline = "Cálculo trigonométrico real derivado del AstroEngine.",
                Sun = $"Tu Sol se encuentra en el signo de {sunSign}.",
                Moon = $"Tu Luna se sitúa en {moonSign}.",
                Ascendant = $"Tu Ascendente calculado es {ascendantSign}."
            }
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
