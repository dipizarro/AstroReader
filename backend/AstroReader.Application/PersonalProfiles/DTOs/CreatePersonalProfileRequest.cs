namespace AstroReader.Application.PersonalProfiles.DTOs;

public record CreatePersonalProfileRequest : PersonalProfileWriteRequest
{
    public Guid? UserId { get; init; }
    public Guid? SavedChartId { get; init; }
}
