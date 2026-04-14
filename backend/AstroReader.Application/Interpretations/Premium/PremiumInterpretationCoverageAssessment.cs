using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Interpretations.Premium;

internal sealed record PremiumInterpretationCoverageAssessment(
    IReadOnlyList<string> CoveredEntries,
    IReadOnlyList<string> MissingEntries)
{
    public bool IsComplete => MissingEntries.Count == 0;

    public bool HasAnyCoverage => CoveredEntries.Count > 0;

    public InterpretationCoverage ToDto(
        InterpretationCoverageStatus status,
        IEnumerable<string>? composedBlocks = null)
    {
        return new InterpretationCoverage
        {
            CoverageStatus = status.ToApiValue(),
            CoveredEntries = CoveredEntries.ToList(),
            MissingEntries = MissingEntries.ToList(),
            ComposedBlocks = composedBlocks?.ToList() ?? []
        };
    }
}
