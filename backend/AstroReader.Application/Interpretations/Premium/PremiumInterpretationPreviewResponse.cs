using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Interpretations.Premium;

public sealed record PremiumInterpretationPreviewResponse
{
    public PremiumInterpretationPreviewSelection Selection { get; init; } = new();

    public InterpretationAnalysisResult Analysis { get; init; } = new();

    public ChartInterpretation Interpretation { get; init; } = new();
}

public sealed record PremiumInterpretationPreviewSelection
{
    public string Sun { get; init; } = string.Empty;

    public string Moon { get; init; } = string.Empty;

    public string Ascendant { get; init; } = string.Empty;

    public string Mercury { get; init; } = string.Empty;

    public string Venus { get; init; } = string.Empty;

    public string Mars { get; init; } = string.Empty;
}
