using System;
using System.Collections.Generic;

namespace AstroReader.Application.Charts.DTOs;

public record CalculateChartResponse
{
    public ChartMetadata Metadata { get; init; } = new(); 
    public ChartSummary Summary { get; init; } = new();
    public List<PlanetPositionDto> Planets { get; init; } = [];
    public List<HousePositionDto> Houses { get; init; } = [];
    public ChartInterpretation Interpretation { get; init; } = new();
}

public record ChartMetadata
{
    public DateTime CalculatedForUtc { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? PlaceName { get; init; }
}

public record ChartSummary
{
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
}

public record PlanetPositionDto
{
    public string Name { get; init; } = string.Empty;
    public string Sign { get; init; } = string.Empty;
    public double SignDegree { get; init; }     
    public double AbsoluteDegree { get; init; } 
    public bool IsRetrograde { get; init; }     
}

public record HousePositionDto
{
    public int Number { get; init; }
    public string Sign { get; init; } = string.Empty;
    public double AbsoluteDegree { get; init; }
}

public record ChartInterpretation
{
    public string Headline { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string GeneralSummary { get; init; } = string.Empty;
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
    public string Mercury { get; init; } = string.Empty;
    public string Venus { get; init; } = string.Empty;
    public string Mars { get; init; } = string.Empty;
    public CoreInterpretation Core { get; init; } = new();
    public PersonalPlanetsInterpretation PersonalPlanets { get; init; } = new();
    public List<HouseInterpretationDto> Houses { get; init; } = [];
    public List<InterpretationProfileDto> Profiles { get; init; } = [];
}

public record CoreInterpretation
{
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
}

public record PersonalPlanetsInterpretation
{
    public string Mercury { get; init; } = string.Empty;
    public string Venus { get; init; } = string.Empty;
    public string Mars { get; init; } = string.Empty;
}

public record HouseInterpretationDto
{
    public int HouseNumber { get; init; }
    public string Sign { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Meaning { get; init; } = string.Empty;
}

public record InterpretationProfileDto
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
}
