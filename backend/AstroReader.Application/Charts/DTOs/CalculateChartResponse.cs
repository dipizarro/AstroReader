namespace AstroReader.Application.Charts.DTOs;

public record CalculateChartResponse
{
    public ChartSummary Summary { get; init; } = new();
    public List<PlanetPosition> Planets { get; init; } = [];
    public List<HousePosition> Houses { get; init; } = [];
    public ChartInterpretation Interpretation { get; init; } = new();
}

public record ChartSummary
{
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
}

public record PlanetPosition
{
    public string Name { get; init; } = string.Empty;
    public string Sign { get; init; } = string.Empty;
    public double Degree { get; init; }
}

public record HousePosition
{
    public int House { get; init; }
    public string Sign { get; init; } = string.Empty;
}

public record ChartInterpretation
{
    public string Headline { get; init; } = string.Empty;
    public string Sun { get; init; } = string.Empty;
    public string Moon { get; init; } = string.Empty;
    public string Ascendant { get; init; } = string.Empty;
}
