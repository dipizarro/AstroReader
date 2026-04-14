using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationCoverageEvaluator
{
    public static PremiumInterpretationCoverageAssessment Evaluate(
        PremiumInterpretationCatalog catalog,
        ZodiacSign sun,
        ZodiacSign moon,
        ZodiacSign ascendant,
        ZodiacSign mercury,
        ZodiacSign venus,
        ZodiacSign mars)
    {
        var requiredEntries = new[]
        {
            CreateRequirement(PremiumInterpretationPosition.Sun, sun),
            CreateRequirement(PremiumInterpretationPosition.Moon, moon),
            CreateRequirement(PremiumInterpretationPosition.Ascendant, ascendant),
            CreateRequirement(PremiumInterpretationPosition.Mercury, mercury),
            CreateRequirement(PremiumInterpretationPosition.Venus, venus),
            CreateRequirement(PremiumInterpretationPosition.Mars, mars)
        };

        var coveredEntries = requiredEntries
            .Where(x => catalog.HasEntry(x.Position, x.Sign))
            .Select(x => x.Key)
            .ToList();

        var missingEntries = requiredEntries
            .Where(x => !catalog.HasEntry(x.Position, x.Sign))
            .Select(x => x.Key)
            .ToList();

        return new PremiumInterpretationCoverageAssessment(coveredEntries, missingEntries);
    }

    private static PremiumEntryRequirement CreateRequirement(
        PremiumInterpretationPosition position,
        ZodiacSign sign)
    {
        return new PremiumEntryRequirement(
            position,
            sign,
            $"{position.ToString().ToLowerInvariant()}.{sign.ToString().ToLowerInvariant()}");
    }

    private sealed record PremiumEntryRequirement(
        PremiumInterpretationPosition Position,
        ZodiacSign Sign,
        string Key);
}
