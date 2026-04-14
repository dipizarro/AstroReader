namespace AstroReader.Application.Interpretations.Premium;

internal enum InterpretationCoverageStatus
{
    Complete,
    Partial,
    Fallback
}

internal static class InterpretationCoverageStatusExtensions
{
    public static string ToApiValue(this InterpretationCoverageStatus status)
    {
        return status switch
        {
            InterpretationCoverageStatus.Complete => "complete",
            InterpretationCoverageStatus.Partial => "partial",
            _ => "fallback"
        };
    }
}
