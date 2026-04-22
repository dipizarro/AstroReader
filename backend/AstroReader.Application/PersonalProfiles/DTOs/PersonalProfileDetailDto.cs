namespace AstroReader.Application.PersonalProfiles.DTOs;

public record PersonalProfileDetailDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public Guid? SavedChartId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string BirthDate { get; init; } = string.Empty;
    public string BirthTime { get; init; } = string.Empty;
    public DateTime BirthInstantUtc { get; init; }
    public string BirthPlace { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int TimezoneOffsetMinutes { get; init; }
    public string SelfPerceptionFocus { get; init; } = string.Empty;
    public string CurrentChallenge { get; init; } = string.Empty;
    public string DesiredInsight { get; init; } = string.Empty;
    public string? SelfDescription { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
