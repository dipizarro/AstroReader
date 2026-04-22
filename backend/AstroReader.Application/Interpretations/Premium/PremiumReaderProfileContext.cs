using AstroReader.Domain.Entities;

namespace AstroReader.Application.Interpretations.Premium;

public sealed record PremiumReaderProfileContext
{
    public Guid ProfileId { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string SelfPerceptionFocus { get; init; } = string.Empty;

    public string CurrentChallenge { get; init; } = string.Empty;

    public string DesiredInsight { get; init; } = string.Empty;

    public string? SelfDescription { get; init; }

    public bool HasEditorialContext =>
        !string.IsNullOrWhiteSpace(SelfPerceptionFocus) ||
        !string.IsNullOrWhiteSpace(CurrentChallenge) ||
        !string.IsNullOrWhiteSpace(DesiredInsight) ||
        !string.IsNullOrWhiteSpace(SelfDescription);

    public static PremiumReaderProfileContext FromPersonalProfile(PersonalProfile profile)
    {
        return new PremiumReaderProfileContext
        {
            ProfileId = profile.Id,
            FullName = profile.FullName,
            DisplayName = ResolveDisplayName(profile.FullName),
            SelfPerceptionFocus = profile.SelfPerceptionFocus,
            CurrentChallenge = profile.CurrentChallenge,
            DesiredInsight = profile.DesiredInsight,
            SelfDescription = profile.SelfDescription
        };
    }

    private static string ResolveDisplayName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return string.Empty;
        }

        var normalized = fullName.Trim();
        var firstSegment = normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        return string.IsNullOrWhiteSpace(firstSegment)
            ? normalized
            : firstSegment;
    }
}
