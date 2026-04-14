using AstroReader.Application.Charts.DTOs;

namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationResponseMapper
{
    public static readonly IReadOnlyList<string> PrimaryComposedBlocks =
    [
        "energyCore",
        "core",
        "personalDynamics",
        "essentialSummary"
    ];

    public static ChartInterpretation MapComposition(
        PremiumInterpretationCompositionResult composition,
        InterpretationCoverage coverage)
    {
        return new ChartInterpretation
        {
            Coverage = coverage,
            Hook = composition.Hook,
            EnergyCore = MapBlock(composition.CentralEnergy),
            Core = MapBlock(composition.Core),
            PersonalDynamics = MapBlock(composition.ThinkingRelatingActing),
            EssentialSummary = MapBlock(composition.Essential),
            TensionsAndPotential = [],
            LifeAreas = [],
            Profiles = [],
            Closing = composition.Closing
        };
    }

    private static InterpretationContentBlock MapBlock(PremiumInterpretationBlock block)
    {
        return new InterpretationContentBlock
        {
            Key = block.Key,
            Title = block.Title,
            MainText = block.Summary,
            SubBlocks = block.Paragraphs
                .Select((paragraph, index) => new InterpretationSubBlock
                {
                    Key = $"{block.Key}-paragraph-{index + 1}",
                    Title = string.Empty,
                    Text = paragraph
                })
                .ToList()
        };
    }
}
