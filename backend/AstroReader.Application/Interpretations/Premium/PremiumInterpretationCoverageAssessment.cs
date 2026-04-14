using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Interpretations.Premium;

public sealed record PremiumInterpretationCoverageAssessment(
    IReadOnlyList<string> CoveredEntries,
    IReadOnlyList<string> MissingEntries)
{
    public bool IsComplete => MissingEntries.Count == 0;

    public bool HasAnyCoverage => CoveredEntries.Count > 0;

    internal InterpretationCoverage ToDto(
        InterpretationCoverageStatus status,
        IEnumerable<string>? composedBlocks = null)
    {
        return new InterpretationCoverage
        {
            CoverageStatus = status.ToApiValue(),
            IsPremiumResult = status != InterpretationCoverageStatus.Fallback,
            IsFallback = status == InterpretationCoverageStatus.Fallback,
            CoveredEntries = CoveredEntries.ToList(),
            MissingEntries = MissingEntries.ToList(),
            ComposedBlocks = composedBlocks?.ToList() ?? []
        };
    }
}
