namespace AstroReader.Application.SavedCharts.DTOs;

public record SavedChartListItemDto
{
    public Guid Id { get; init; }
    public string ProfileName { get; init; } = string.Empty;
    public string? PlaceName { get; init; }
    public string BirthDate { get; init; } = string.Empty;
    public string BirthTime { get; init; } = string.Empty;
    public int TimezoneOffsetMinutes { get; init; }
    public string SunSign { get; init; } = string.Empty;
    public string MoonSign { get; init; } = string.Empty;
    public string AscendantSign { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
