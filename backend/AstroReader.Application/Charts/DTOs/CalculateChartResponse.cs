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
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
}
