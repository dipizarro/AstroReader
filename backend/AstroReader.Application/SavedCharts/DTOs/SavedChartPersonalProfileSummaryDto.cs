namespace AstroReader.Application.SavedCharts.DTOs;

public record SavedChartPersonalProfileSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string SelfPerceptionFocus { get; init; } = string.Empty;
    public string CurrentChallenge { get; init; } = string.Empty;
    public string DesiredInsight { get; init; } = string.Empty;
}
