namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationComposer : IInterpretationComposer
{
    public PremiumInterpretationCompositionResult Compose(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var plan = PremiumInterpretationContentSelector.CreatePlan(context, analysis);

        return new PremiumInterpretationCompositionResult
        {
            Hook = plan.Hook,
            CentralEnergy = BuildBlock(plan.EnergyCore),
            Core = BuildBlock(plan.Core),
            ThinkingRelatingActing = BuildBlock(plan.PersonalDynamics),
            Essential = BuildBlock(plan.EssentialSummary),
            Closing = plan.Closing
        };
    }

    private static PremiumInterpretationBlock BuildBlock(PremiumInterpretationBlockSelection selection)
    {
        if (string.IsNullOrWhiteSpace(selection.MainClaim) && selection.SupportingClaims.Count == 0)
        {
            return PremiumInterpretationBlock.Empty(selection.Key, selection.Title);
        }

        return new PremiumInterpretationBlock
        {
            Key = selection.Key,
            Title = selection.Title,
            Summary = selection.MainClaim,
            Paragraphs = selection.SupportingClaims
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            Highlights = selection.Highlights
        };
    }
}
