namespace AstroReader.Application.Interpretations.Premium;

public sealed class InterpretationAnalysisResult
{
    public AnalysisInsight DominantCoreTrait { get; init; } = AnalysisInsight.Empty("dominantCoreTrait");

    public AnalysisInsight EmotionalTone { get; init; } = AnalysisInsight.Empty("emotionalTone");

    public AnalysisInsight RelationalStyle { get; init; } = AnalysisInsight.Empty("relationalStyle");

    public AnalysisInsight ActionStyle { get; init; } = AnalysisInsight.Empty("actionStyle");

    public AnalysisInsight CentralTension { get; init; } = AnalysisInsight.Empty("centralTension");

    public AnalysisInsight GrowthDirection { get; init; } = AnalysisInsight.Empty("growthDirection");
}

public sealed class AnalysisInsight
{
    public string Key { get; init; } = string.Empty;

    public string Headline { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public IReadOnlyList<string> Keywords { get; init; } = [];

    public IReadOnlyList<string> Signals { get; init; } = [];

    public IReadOnlyList<PremiumInterpretationPosition> SourcePositions { get; init; } = [];

    public static AnalysisInsight Empty(string key)
    {
        return new AnalysisInsight
        {
            Key = key
        };
    }
}
