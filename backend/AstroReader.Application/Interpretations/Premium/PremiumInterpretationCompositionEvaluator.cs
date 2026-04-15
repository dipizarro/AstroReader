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
        if (context.HasCompleteCoverage && composedBlocks.Count == 4)
        {
            return InterpretationCoverageStatus.Complete;
        }

        if (HasSubstantivePartialCoverage(context, composedBlocks))
        {
            return InterpretationCoverageStatus.Partial;
        }

        return InterpretationCoverageStatus.Fallback;
    }

    private static bool HasContent(PremiumInterpretationBlock block)
    {
        return !string.IsNullOrWhiteSpace(block.Summary) || block.Paragraphs.Count > 0;
    }

    private static bool HasSubstantivePartialCoverage(
        PremiumInterpretationContext context,
        IReadOnlyList<string> composedBlocks)
    {
        var coreCoverageCount = CountAvailable(context.Sun, context.Moon, context.Ascendant);
        var personalDynamicsCoverageCount = CountAvailable(context.Mercury, context.Venus, context.Mars);
        var hasCoreComposition = composedBlocks.Contains("energyCore") || composedBlocks.Contains("core");
        var hasPersonalDynamicsComposition = composedBlocks.Contains("personalDynamics");

        return
            (coreCoverageCount >= 2 && hasCoreComposition) ||
            (personalDynamicsCoverageCount >= 2 && hasPersonalDynamicsComposition && composedBlocks.Count >= 2);
    }

    private static int CountAvailable(params InterpretationEntry?[] entries)
    {
        return entries.Count(entry => entry is not null);
    }
}
