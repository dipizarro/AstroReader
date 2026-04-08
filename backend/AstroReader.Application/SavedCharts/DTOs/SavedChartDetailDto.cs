using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.SavedCharts.DTOs;

public record SavedChartDetailDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public string ProfileName { get; init; } = string.Empty;
    public string? PlaceName { get; init; }
    public string BirthDate { get; init; } = string.Empty;
    public string BirthTime { get; init; } = string.Empty;
    public int TimezoneOffsetMinutes { get; init; }
    public DateTime BirthInstantUtc { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string SunSign { get; init; } = string.Empty;
    public string MoonSign { get; init; } = string.Empty;
    public string AscendantSign { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public CalculateChartResponse Chart { get; init; } = new();
}
