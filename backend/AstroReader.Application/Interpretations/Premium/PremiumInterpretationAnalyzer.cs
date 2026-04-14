namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationAnalyzer : IInterpretationAnalyzer
{
    public InterpretationAnalysisResult Analyze(PremiumInterpretationContext context)
    {
        var entries = new ChartSemanticEntries(
            Sun: context.RequireSun(),
            Moon: context.RequireMoon(),
            Ascendant: context.RequireAscendant(),
            Mercury: context.RequireMercury(),
            Venus: context.RequireVenus(),
            Mars: context.RequireMars());

        return new InterpretationAnalysisResult
        {
            DominantCoreTrait = BuildDominantCoreTrait(entries),
            EmotionalTone = BuildEmotionalTone(entries),
            RelationalStyle = BuildRelationalStyle(entries),
            ActionStyle = BuildActionStyle(entries),
            CentralTension = BuildCentralTension(entries),
            GrowthDirection = BuildGrowthDirection(entries)
        };
    }

    private static AnalysisInsight BuildDominantCoreTrait(ChartSemanticEntries entries)
    {
        var dominantKeyword = SelectDominantKeyword(entries.Sun.Keywords, entries.Moon.Keywords, entries.Ascendant.Keywords);

        return new AnalysisInsight
        {
            Key = "dominantCoreTrait",
            Headline = $"Núcleo dominante: {ToDisplayLabel(dominantKeyword)}",
            Summary =
                $"{entries.Sun.IdentityStyle} {entries.Moon.EmotionalStyle} {entries.Ascendant.OuterStyle}",
            Keywords = TakeDistinctKeywords(entries.Sun.Keywords, entries.Moon.Keywords, entries.Ascendant.Keywords),
            Signals =
            [
                entries.Sun.IdentityStyle,
                entries.Moon.EmotionalStyle,
                entries.Ascendant.OuterStyle
            ],
            SourcePositions = [PremiumInterpretationPosition.Sun, PremiumInterpretationPosition.Moon, PremiumInterpretationPosition.Ascendant]
        };
    }

    private static AnalysisInsight BuildEmotionalTone(ChartSemanticEntries entries)
    {
        return new AnalysisInsight
        {
            Key = "emotionalTone",
            Headline = "Tono emocional principal",
            Summary =
                $"{entries.Moon.EmotionalStyle} {entries.Moon.EmotionalNeeds} {entries.Moon.SecurityNeeds}",
            Keywords = TakeDistinctKeywords(entries.Moon.Keywords),
            Signals =
            [
                entries.Moon.EmotionalStyle,
                entries.Moon.EmotionalNeeds,
                entries.Moon.SecurityNeeds
            ],
            SourcePositions = [PremiumInterpretationPosition.Moon]
        };
    }

    private static AnalysisInsight BuildRelationalStyle(ChartSemanticEntries entries)
    {
        return new AnalysisInsight
        {
            Key = "relationalStyle",
            Headline = "Forma general de vincularse",
            Summary =
                $"{entries.Venus.RelationalStyle} {entries.Venus.AffectiveNeeds} {entries.Ascendant.SocialStyle} {entries.Mercury.CommunicationStyle}",
            Keywords = TakeDistinctKeywords(entries.Venus.Keywords, entries.Ascendant.Keywords, entries.Mercury.Keywords),
            Signals =
            [
                entries.Venus.RelationalStyle,
                entries.Venus.AffectiveNeeds,
                entries.Ascendant.SocialStyle,
                entries.Mercury.CommunicationStyle
            ],
            SourcePositions = [PremiumInterpretationPosition.Venus, PremiumInterpretationPosition.Ascendant, PremiumInterpretationPosition.Mercury]
        };
    }

    private static AnalysisInsight BuildActionStyle(ChartSemanticEntries entries)
    {
        return new AnalysisInsight
        {
            Key = "actionStyle",
            Headline = "Forma general de actuar",
            Summary =
                $"{entries.Mars.ActionStyle} {entries.Mars.DesireStyle} {entries.Mercury.ThinkingStyle}",
            Keywords = TakeDistinctKeywords(entries.Mars.Keywords, entries.Mercury.Keywords),
            Signals =
            [
                entries.Mars.ActionStyle,
                entries.Mars.DesireStyle,
                entries.Mercury.ThinkingStyle
            ],
            SourcePositions = [PremiumInterpretationPosition.Mars, PremiumInterpretationPosition.Mercury]
        };
    }

    private static AnalysisInsight BuildCentralTension(ChartSemanticEntries entries)
    {
        var candidates = new[]
        {
            CreateContrastCandidate(
                "identityEmotion",
                "Tensión entre identidad y necesidad emocional",
                entries.Sun.IdentityStyle,
                entries.Moon.EmotionalNeeds,
                entries.Sun.Keywords,
                entries.Moon.Keywords,
                PremiumInterpretationPosition.Sun,
                PremiumInterpretationPosition.Moon),
            CreateContrastCandidate(
                "bondDesire",
                "Tensión entre vínculo y deseo",
                entries.Venus.RelationalStyle,
                entries.Mars.DesireStyle,
                entries.Venus.Keywords,
                entries.Mars.Keywords,
                PremiumInterpretationPosition.Venus,
                PremiumInterpretationPosition.Mars),
            CreateContrastCandidate(
                "mindEmotion",
                "Tensión entre mente y emoción",
                entries.Mercury.ThinkingStyle,
                entries.Moon.EmotionalStyle,
                entries.Mercury.Keywords,
                entries.Moon.Keywords,
                PremiumInterpretationPosition.Mercury,
                PremiumInterpretationPosition.Moon)
        };

        var selected = candidates
            .OrderByDescending(x => x.ContrastScore)
            .ThenBy(x => x.Priority)
            .First();

        return new AnalysisInsight
        {
            Key = "centralTension",
            Headline = selected.Headline,
            Summary = selected.Summary,
            Keywords = selected.Keywords,
            Signals = selected.Signals,
            SourcePositions = selected.SourcePositions
        };
    }

    private static AnalysisInsight BuildGrowthDirection(ChartSemanticEntries entries)
    {
        return new AnalysisInsight
        {
            Key = "growthDirection",
            Headline = "Posible dirección de crecimiento",
            Summary =
                $"{entries.Sun.GrowthPath} {entries.Moon.IntegrationHooks[0]} {entries.Mars.IntegrationHooks[0]}",
            Keywords = TakeDistinctKeywords(entries.Sun.Keywords, entries.Moon.Keywords, entries.Mars.Keywords),
            Signals =
            [
                entries.Sun.GrowthPath,
                entries.Moon.IntegrationHooks[0],
                entries.Mars.IntegrationHooks[0]
            ],
            SourcePositions = [PremiumInterpretationPosition.Sun, PremiumInterpretationPosition.Moon, PremiumInterpretationPosition.Mars]
        };
    }

    private static ContrastCandidate CreateContrastCandidate(
        string key,
        string headline,
        string firstSignal,
        string secondSignal,
        IReadOnlyList<string> firstKeywords,
        IReadOnlyList<string> secondKeywords,
        PremiumInterpretationPosition firstPosition,
        PremiumInterpretationPosition secondPosition)
    {
        var sharedKeywords = firstKeywords
            .Intersect(secondKeywords, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var combinedKeywords = TakeDistinctKeywords(firstKeywords, secondKeywords);
        var contrastScore = combinedKeywords.Count - sharedKeywords.Count;
        var priority = key switch
        {
            "identityEmotion" => 0,
            "bondDesire" => 1,
            _ => 2
        };

        return new ContrastCandidate(
            ContrastScore: contrastScore,
            Priority: priority,
            Headline: headline,
            Summary: $"{firstSignal} {secondSignal}",
            Keywords: combinedKeywords,
            Signals: [firstSignal, secondSignal],
            SourcePositions: [firstPosition, secondPosition]);
    }

    private static string SelectDominantKeyword(params IReadOnlyList<string>[] keywordSets)
    {
        var frequencies = keywordSets
            .SelectMany(x => x)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .GroupBy(x => x.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return frequencies?.Key ?? "centro";
    }

    private static IReadOnlyList<string> TakeDistinctKeywords(params IReadOnlyList<string>[] keywordSets)
    {
        return keywordSets
            .SelectMany(x => x)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToList();
    }

    private static string ToDisplayLabel(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return "Centro";
        }

        return char.ToUpperInvariant(keyword[0]) + keyword[1..];
    }

    private sealed record ChartSemanticEntries(
        SunInterpretationEntry Sun,
        MoonInterpretationEntry Moon,
        AscendantInterpretationEntry Ascendant,
        MercuryInterpretationEntry Mercury,
        VenusInterpretationEntry Venus,
        MarsInterpretationEntry Mars);

    private sealed record ContrastCandidate(
        int ContrastScore,
        int Priority,
        string Headline,
        string Summary,
        IReadOnlyList<string> Keywords,
        IReadOnlyList<string> Signals,
        IReadOnlyList<PremiumInterpretationPosition> SourcePositions);
}
