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
    public InterpretationCoverage Coverage { get; init; } = new();
    public string Hook { get; init; } = string.Empty;
    public InterpretationContentBlock EnergyCore { get; init; } = new();
    public InterpretationContentBlock Core { get; init; } = new();
    public InterpretationContentBlock PersonalDynamics { get; init; } = new();
    public InterpretationContentBlock EssentialSummary { get; init; } = new();
    public List<InterpretationContentBlock> TensionsAndPotential { get; init; } = [];
    public List<InterpretationContentBlock> LifeAreas { get; init; } = [];
    public List<InterpretationProfileDto> Profiles { get; init; } = [];
    public string Closing { get; init; } = string.Empty;
}

public record InterpretationCoverage
{
    public string CoverageStatus { get; init; } = "fallback";
    public List<string> CoveredEntries { get; init; } = [];
    public List<string> MissingEntries { get; init; } = [];
    public List<string> ComposedBlocks { get; init; } = [];
}

public record InterpretationContentBlock
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string MainText { get; init; } = string.Empty;
    public List<InterpretationSubBlock> SubBlocks { get; init; } = [];
}

public record InterpretationSubBlock
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}

public record InterpretationProfileDto
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
}
