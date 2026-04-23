namespace AstroReader.Application.PersonalProfiles.DTOs;

public record PersonalProfileListItemDto
{
    public Guid Id { get; init; }
    public Guid? SavedChartId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string BirthDate { get; init; } = string.Empty;
    public string BirthTime { get; init; } = string.Empty;
    public string BirthPlace { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int TimezoneOffsetMinutes { get; init; }
    public string SelfPerceptionFocus { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
