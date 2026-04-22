namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationCompositionResult
{
    public string Hook { get; init; } = string.Empty;

    public PremiumInterpretationBlock CentralEnergy { get; init; } = PremiumInterpretationBlock.Empty("energyCore", "Tu energía central");

    public PremiumInterpretationBlock Core { get; init; } = PremiumInterpretationBlock.Empty("core", "Tu núcleo");

    public PremiumInterpretationBlock ThinkingRelatingActing { get; init; } = PremiumInterpretationBlock.Empty("thinkingRelatingActing", "Tu forma de pensar, vincularte y actuar");

    public PremiumInterpretationBlock Essential { get; init; } = PremiumInterpretationBlock.Empty("essential", "Lo esencial de tu carta");

    public IReadOnlyList<PremiumInterpretationProfile> Profiles { get; init; } = [];

    public string Closing { get; init; } = string.Empty;
}

public sealed class PremiumInterpretationBlock
{
    public string Key { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<string> Paragraphs { get; init; } = [];

    public IReadOnlyList<string> Highlights { get; init; } = [];

    public static PremiumInterpretationBlock Empty(string key, string title)
    {
        return new PremiumInterpretationBlock
        {
            Key = key,
            Title = title
        };
    }
}

public sealed class PremiumInterpretationProfile
{
    public string Key { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;
}
