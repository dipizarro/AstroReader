namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationComposer : IInterpretationComposer
{
    public PremiumInterpretationCompositionResult Compose(PremiumInterpretationContext context, InterpretationAnalysisResult analysis)
    {
        var entries = new ChartSemanticEntries(
            Sun: context.RequireSun(),
            Moon: context.RequireMoon(),
            Ascendant: context.RequireAscendant(),
            Mercury: context.RequireMercury(),
            Venus: context.RequireVenus(),
            Mars: context.RequireMars());

        return new PremiumInterpretationCompositionResult
        {
            Hook = BuildHook(analysis, entries),
            CentralEnergy = BuildCentralEnergyBlock(analysis, entries),
            Core = BuildCoreBlock(analysis, entries),
            ThinkingRelatingActing = BuildThinkingRelatingActingBlock(analysis, entries),
            Essential = BuildEssentialBlock(analysis, entries),
            Closing = BuildClosing(analysis, entries)
        };
    }

    private static string BuildHook(InterpretationAnalysisResult analysis, ChartSemanticEntries entries)
    {
        return ComposeParagraph(
            analysis.DominantCoreTrait.Headline,
            entries.Sun.Summary,
            entries.Moon.Summary);
    }

    private static PremiumInterpretationBlock BuildCentralEnergyBlock(
        InterpretationAnalysisResult analysis,
        ChartSemanticEntries entries)
    {
        return new PremiumInterpretationBlock
        {
            Key = "centralEnergy",
            Title = "Tu energía central",
            Summary = analysis.DominantCoreTrait.Summary,
            Paragraphs =
            [
                ComposeParagraph(
                    entries.Sun.Summary,
                    entries.Sun.IdentityStyle,
                    analysis.DominantCoreTrait.Summary),
                ComposeParagraph(
                    entries.Sun.GrowthPath,
                    FirstOrEmpty(entries.Sun.IntegrationHooks))
            ],
            Highlights = analysis.DominantCoreTrait.Keywords
        };
    }

    private static PremiumInterpretationBlock BuildCoreBlock(
        InterpretationAnalysisResult analysis,
        ChartSemanticEntries entries)
    {
        return new PremiumInterpretationBlock
        {
            Key = "core",
            Title = "Tu núcleo",
            Summary = ComposeParagraph(
                entries.Moon.Summary,
                entries.Ascendant.Summary),
            Paragraphs =
            [
                ComposeParagraph(
                    entries.Moon.EmotionalStyle,
                    entries.Moon.EmotionalNeeds,
                    entries.Moon.SecurityNeeds),
                ComposeParagraph(
                    entries.Ascendant.OuterStyle,
                    entries.Ascendant.SocialStyle,
                    entries.Ascendant.FirstImpression),
                ComposeParagraph(
                    analysis.CentralTension.Headline,
                    analysis.CentralTension.Summary)
            ],
            Highlights = TakeHighlights(
                analysis.EmotionalTone.Keywords,
                entries.Ascendant.Keywords,
                analysis.CentralTension.Keywords)
        };
    }

    private static PremiumInterpretationBlock BuildThinkingRelatingActingBlock(
        InterpretationAnalysisResult analysis,
        ChartSemanticEntries entries)
    {
        return new PremiumInterpretationBlock
        {
            Key = "thinkingRelatingActing",
            Title = "Tu forma de pensar, vincularte y actuar",
            Summary = ComposeParagraph(
                analysis.RelationalStyle.Summary,
                analysis.ActionStyle.Summary),
            Paragraphs =
            [
                ComposeParagraph(
                    entries.Mercury.Summary,
                    entries.Mercury.ThinkingStyle,
                    entries.Mercury.CommunicationStyle,
                    entries.Mercury.LearningStyle),
                ComposeParagraph(
                    entries.Venus.Summary,
                    entries.Venus.RelationalStyle,
                    entries.Venus.AffectiveNeeds),
                ComposeParagraph(
                    entries.Mars.Summary,
                    entries.Mars.ActionStyle,
                    entries.Mars.DesireStyle,
                    entries.Mars.ConflictStyle)
            ],
            Highlights = TakeHighlights(
                entries.Mercury.Keywords,
                entries.Venus.Keywords,
                entries.Mars.Keywords)
        };
    }

    private static PremiumInterpretationBlock BuildEssentialBlock(
        InterpretationAnalysisResult analysis,
        ChartSemanticEntries entries)
    {
        return new PremiumInterpretationBlock
        {
            Key = "essential",
            Title = "Lo esencial de tu carta",
            Summary = ComposeParagraph(
                analysis.GrowthDirection.Headline,
                analysis.GrowthDirection.Summary),
            Paragraphs =
            [
                ComposeParagraph(
                    analysis.DominantCoreTrait.Summary,
                    analysis.EmotionalTone.Summary),
                ComposeParagraph(
                    analysis.RelationalStyle.Summary,
                    analysis.ActionStyle.Summary),
                ComposeParagraph(
                    entries.Sun.GrowthPath,
                    FirstOrEmpty(entries.Moon.IntegrationHooks),
                    FirstOrEmpty(entries.Mars.IntegrationHooks))
            ],
            Highlights = TakeHighlights(
                analysis.DominantCoreTrait.Keywords,
                analysis.GrowthDirection.Keywords)
        };
    }

    private static string BuildClosing(
        InterpretationAnalysisResult analysis,
        ChartSemanticEntries entries)
    {
        return ComposeParagraph(
            "La fuerza de esta carta no está en elegir una sola versión de ti, sino en ordenar tus distintas capas en una dirección consciente.",
            analysis.GrowthDirection.Summary,
            FirstOrEmpty(entries.Sun.IntegrationHooks));
    }

    private static string ComposeParagraph(params string[] fragments)
    {
        var normalized = fragments
            .Select(NormalizeText)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return string.Join(" ", normalized);
    }

    private static IReadOnlyList<string> TakeHighlights(params IReadOnlyList<string>[] keywordSets)
    {
        return keywordSets
            .SelectMany(x => x)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToList();
    }

    private static string FirstOrEmpty(IReadOnlyList<string> values)
    {
        return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
    }

    private static string NormalizeText(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }

    private sealed record ChartSemanticEntries(
        SunInterpretationEntry Sun,
        MoonInterpretationEntry Moon,
        AscendantInterpretationEntry Ascendant,
        MercuryInterpretationEntry Mercury,
        VenusInterpretationEntry Venus,
        MarsInterpretationEntry Mars);
}
