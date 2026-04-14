namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationAnalyzer : IInterpretationAnalyzer
{
    public InterpretationAnalysisResult Analyze(PremiumInterpretationContext context)
    {
        return new InterpretationAnalysisResult
        {
            DominantCoreTrait = BuildDominantCoreTrait(context),
            EmotionalTone = BuildEmotionalTone(context),
            RelationalStyle = BuildRelationalStyle(context),
            ActionStyle = BuildActionStyle(context),
            CentralTension = BuildCentralTension(context),
            GrowthDirection = BuildGrowthDirection(context)
        };
    }

    private static AnalysisInsight BuildDominantCoreTrait(PremiumInterpretationContext context)
    {
        if (context.Sun is null || context.Moon is null || context.Ascendant is null)
        {
            return AnalysisInsight.Empty("dominantCoreTrait");
        }

        var dominantKeyword = SelectDominantKeyword(context.Sun.Keywords, context.Moon.Keywords, context.Ascendant.Keywords);

        return new AnalysisInsight
        {
            Key = "dominantCoreTrait",
            Headline = $"Núcleo dominante: {ToDisplayLabel(dominantKeyword)}",
            Summary =
                $"{context.Sun.IdentityStyle} {context.Moon.EmotionalStyle} {context.Ascendant.OuterStyle}",
            Keywords = TakeDistinctKeywords(context.Sun.Keywords, context.Moon.Keywords, context.Ascendant.Keywords),
            Signals =
            [
                context.Sun.IdentityStyle,
                context.Moon.EmotionalStyle,
                context.Ascendant.OuterStyle
            ],
            SourcePositions = [PremiumInterpretationPosition.Sun, PremiumInterpretationPosition.Moon, PremiumInterpretationPosition.Ascendant]
        };
    }

    private static AnalysisInsight BuildEmotionalTone(PremiumInterpretationContext context)
    {
        if (context.Moon is null)
        {
            return AnalysisInsight.Empty("emotionalTone");
        }

        return new AnalysisInsight
        {
            Key = "emotionalTone",
            Headline = "Tono emocional principal",
            Summary =
                $"{context.Moon.EmotionalStyle} {context.Moon.EmotionalNeeds} {context.Moon.SecurityNeeds}",
            Keywords = TakeDistinctKeywords(context.Moon.Keywords),
            Signals =
            [
                context.Moon.EmotionalStyle,
                context.Moon.EmotionalNeeds,
                context.Moon.SecurityNeeds
            ],
            SourcePositions = [PremiumInterpretationPosition.Moon]
        };
    }

    private static AnalysisInsight BuildRelationalStyle(PremiumInterpretationContext context)
    {
        var signals = new List<string>();
        var keywordSets = new List<IReadOnlyList<string>>();
        var sourcePositions = new List<PremiumInterpretationPosition>();

        if (context.Venus is not null)
        {
            signals.Add(context.Venus.RelationalStyle);
            signals.Add(context.Venus.AffectiveNeeds);
            keywordSets.Add(context.Venus.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Venus);
        }

        if (context.Ascendant is not null)
        {
            signals.Add(context.Ascendant.SocialStyle);
            keywordSets.Add(context.Ascendant.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Ascendant);
        }

        if (context.Mercury is not null)
        {
            signals.Add(context.Mercury.CommunicationStyle);
            keywordSets.Add(context.Mercury.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Mercury);
        }

        var filteredSignals = signals
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (filteredSignals.Count == 0)
        {
            return AnalysisInsight.Empty("relationalStyle");
        }

        return new AnalysisInsight
        {
            Key = "relationalStyle",
            Headline = "Forma general de vincularse",
            Summary = string.Join(" ", filteredSignals),
            Keywords = TakeDistinctKeywords(keywordSets.ToArray()),
            Signals = filteredSignals,
            SourcePositions = sourcePositions.Distinct().ToList()
        };
    }

    private static AnalysisInsight BuildActionStyle(PremiumInterpretationContext context)
    {
        var signals = new List<string>();
        var keywordSets = new List<IReadOnlyList<string>>();
        var sourcePositions = new List<PremiumInterpretationPosition>();

        if (context.Mars is not null)
        {
            signals.Add(context.Mars.ActionStyle);
            signals.Add(context.Mars.DesireStyle);
            keywordSets.Add(context.Mars.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Mars);
        }

        if (context.Mercury is not null)
        {
            signals.Add(context.Mercury.ThinkingStyle);
            keywordSets.Add(context.Mercury.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Mercury);
        }

        var filteredSignals = signals
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (filteredSignals.Count == 0)
        {
            return AnalysisInsight.Empty("actionStyle");
        }

        return new AnalysisInsight
        {
            Key = "actionStyle",
            Headline = "Forma general de actuar",
            Summary = string.Join(" ", filteredSignals),
            Keywords = TakeDistinctKeywords(keywordSets.ToArray()),
            Signals = filteredSignals,
            SourcePositions = sourcePositions.Distinct().ToList()
        };
    }

    private static AnalysisInsight BuildCentralTension(PremiumInterpretationContext context)
    {
        var candidates = new List<ContrastCandidate>();

        if (context.Sun is not null && context.Moon is not null)
        {
            candidates.Add(CreateContrastCandidate(
                "identityEmotion",
                "Tensión entre identidad y necesidad emocional",
                context.Sun.IdentityStyle,
                context.Moon.EmotionalNeeds,
                context.Sun.Keywords,
                context.Moon.Keywords,
                PremiumInterpretationPosition.Sun,
                PremiumInterpretationPosition.Moon));
        }

        if (context.Venus is not null && context.Mars is not null)
        {
            candidates.Add(CreateContrastCandidate(
                "bondDesire",
                "Tensión entre vínculo y deseo",
                context.Venus.RelationalStyle,
                context.Mars.DesireStyle,
                context.Venus.Keywords,
                context.Mars.Keywords,
                PremiumInterpretationPosition.Venus,
                PremiumInterpretationPosition.Mars));
        }

        if (context.Mercury is not null && context.Moon is not null)
        {
            candidates.Add(CreateContrastCandidate(
                "mindEmotion",
                "Tensión entre mente y emoción",
                context.Mercury.ThinkingStyle,
                context.Moon.EmotionalStyle,
                context.Mercury.Keywords,
                context.Moon.Keywords,
                PremiumInterpretationPosition.Mercury,
                PremiumInterpretationPosition.Moon));
        }

        if (candidates.Count == 0)
        {
            return AnalysisInsight.Empty("centralTension");
        }

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

    private static AnalysisInsight BuildGrowthDirection(PremiumInterpretationContext context)
    {
        if (context.Sun is null)
        {
            return AnalysisInsight.Empty("growthDirection");
        }

        var signals = new List<string> { context.Sun.GrowthPath };
        var keywordSets = new List<IReadOnlyList<string>> { context.Sun.Keywords };
        var sourcePositions = new List<PremiumInterpretationPosition> { PremiumInterpretationPosition.Sun };

        var moonHook = context.Moon?.IntegrationHooks.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (!string.IsNullOrWhiteSpace(moonHook))
        {
            signals.Add(moonHook);
            keywordSets.Add(context.Moon!.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Moon);
        }

        var marsHook = context.Mars?.IntegrationHooks.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (!string.IsNullOrWhiteSpace(marsHook))
        {
            signals.Add(marsHook);
            keywordSets.Add(context.Mars!.Keywords);
            sourcePositions.Add(PremiumInterpretationPosition.Mars);
        }

        return new AnalysisInsight
        {
            Key = "growthDirection",
            Headline = "Posible dirección de crecimiento",
            Summary = string.Join(" ", signals.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase)),
            Keywords = TakeDistinctKeywords(keywordSets.ToArray()),
            Signals = signals.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            SourcePositions = sourcePositions.Distinct().ToList()
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

    private sealed record ContrastCandidate(
        int ContrastScore,
        int Priority,
        string Headline,
        string Summary,
        IReadOnlyList<string> Keywords,
        IReadOnlyList<string> Signals,
        IReadOnlyList<PremiumInterpretationPosition> SourcePositions);
}
