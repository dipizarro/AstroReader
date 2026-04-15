namespace AstroReader.Application.Interpretations.Premium;

internal sealed class PremiumInterpretationContentSelectionPlan
{
    public string Hook { get; init; } = string.Empty;

    public PremiumInterpretationBlockSelection EnergyCore { get; init; } =
        PremiumInterpretationBlockSelection.Empty("centralEnergy", "Tu energía central");

    public PremiumInterpretationBlockSelection Core { get; init; } =
        PremiumInterpretationBlockSelection.Empty("core", "Tu núcleo");

    public PremiumInterpretationBlockSelection PersonalDynamics { get; init; } =
        PremiumInterpretationBlockSelection.Empty("thinkingRelatingActing", "Tu forma de pensar, vincularte y actuar");

    public PremiumInterpretationBlockSelection EssentialSummary { get; init; } =
        PremiumInterpretationBlockSelection.Empty("essential", "Lo esencial de tu carta");

    public string Closing { get; init; } = string.Empty;
}

internal sealed class PremiumInterpretationBlockSelection
{
    public string Key { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string MainClaim { get; init; } = string.Empty;

    public IReadOnlyList<string> SupportingClaims { get; init; } = [];

    public IReadOnlyList<string> Highlights { get; init; } = [];

    public static PremiumInterpretationBlockSelection Empty(string key, string title)
    {
        return new PremiumInterpretationBlockSelection
        {
            Key = key,
            Title = title
        };
    }
}
