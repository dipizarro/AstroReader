namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationCompositionEvaluator
{
    public static IReadOnlyList<string> GetComposedBlocks(PremiumInterpretationCompositionResult composition)
    {
        var blocks = new List<string>();

        if (HasContent(composition.CentralEnergy))
        {
            blocks.Add("energyCore");
        }

        if (HasContent(composition.Core))
        {
            blocks.Add("core");
        }

        if (HasContent(composition.ThinkingRelatingActing))
        {
            blocks.Add("personalDynamics");
        }

        if (HasContent(composition.Essential))
        {
            blocks.Add("essentialSummary");
        }

        return blocks;
    }

    public static InterpretationCoverageStatus DetermineStatus(
        PremiumInterpretationContext context,
        IReadOnlyList<string> composedBlocks)
    {
        var hasCoreLayer = composedBlocks.Contains("energyCore") || composedBlocks.Contains("core");

        if (context.HasCompleteCoverage && composedBlocks.Count == 4)
        {
            return InterpretationCoverageStatus.Complete;
        }

        if (hasCoreLayer || composedBlocks.Count >= 2)
        {
            return InterpretationCoverageStatus.Partial;
        }

        return InterpretationCoverageStatus.Fallback;
    }

    private static bool HasContent(PremiumInterpretationBlock block)
    {
        return !string.IsNullOrWhiteSpace(block.Summary) || block.Paragraphs.Count > 0;
    }
}
