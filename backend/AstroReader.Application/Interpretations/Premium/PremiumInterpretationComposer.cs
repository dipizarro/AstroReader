namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationComposer : IInterpretationComposer
{
    public PremiumInterpretationCompositionResult Compose(PremiumInterpretationContext context, InterpretationAnalysisResult analysis)
    {
        return new PremiumInterpretationCompositionResult
        {
            Hook = BuildHook(context, analysis),
            CentralEnergy = BuildCentralEnergyBlock(context, analysis),
            Core = BuildCoreBlock(context, analysis),
            ThinkingRelatingActing = BuildThinkingRelatingActingBlock(context, analysis),
            Essential = BuildEssentialBlock(context, analysis),
            Closing = BuildClosing(context, analysis)
        };
    }

    private static string BuildHook(PremiumInterpretationContext context, InterpretationAnalysisResult analysis)
    {
        if (!string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline))
        {
            return ComposeParagraph(
                analysis.DominantCoreTrait.Headline,
                context.Sun?.Summary ?? string.Empty,
                context.Moon?.Summary ?? string.Empty);
        }

        return ComposeParagraph(
            context.Sun?.Summary ?? string.Empty,
            context.Moon?.Summary ?? string.Empty,
            context.Ascendant?.Summary ?? string.Empty);
    }

    private static PremiumInterpretationBlock BuildCentralEnergyBlock(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (context.Sun is null)
        {
            return PremiumInterpretationBlock.Empty("centralEnergy", "Tu energía central");
        }

        var summary = !string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Summary)
            ? analysis.DominantCoreTrait.Summary
            : ComposeParagraph(
                context.Sun.Summary,
                context.Sun.IdentityStyle,
                context.Moon?.EmotionalStyle ?? string.Empty,
                context.Ascendant?.OuterStyle ?? string.Empty);

        var paragraphs = new List<string>
        {
            ComposeParagraph(
                context.Sun.Summary,
                context.Sun.IdentityStyle,
                analysis.DominantCoreTrait.Summary)
        };

        var growthParagraph = ComposeParagraph(
            context.Sun.GrowthPath,
            FirstOrEmpty(context.Sun.IntegrationHooks));

        if (!string.IsNullOrWhiteSpace(growthParagraph))
        {
            paragraphs.Add(growthParagraph);
        }

        return new PremiumInterpretationBlock
        {
            Key = "centralEnergy",
            Title = "Tu energía central",
            Summary = summary,
            Paragraphs = paragraphs.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = analysis.DominantCoreTrait.Keywords.Count > 0
                ? analysis.DominantCoreTrait.Keywords
                : context.Sun.Keywords
        };
    }

    private static PremiumInterpretationBlock BuildCoreBlock(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (context.Moon is null && context.Ascendant is null)
        {
            return PremiumInterpretationBlock.Empty("core", "Tu núcleo");
        }

        var paragraphs = new List<string>();

        if (context.Moon is not null)
        {
            paragraphs.Add(ComposeParagraph(
                context.Moon.EmotionalStyle,
                context.Moon.EmotionalNeeds,
                context.Moon.SecurityNeeds));
        }

        if (context.Ascendant is not null)
        {
            paragraphs.Add(ComposeParagraph(
                context.Ascendant.OuterStyle,
                context.Ascendant.SocialStyle,
                context.Ascendant.FirstImpression));
        }

        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Summary))
        {
            paragraphs.Add(ComposeParagraph(
                analysis.CentralTension.Headline,
                analysis.CentralTension.Summary));
        }

        return new PremiumInterpretationBlock
        {
            Key = "core",
            Title = "Tu núcleo",
            Summary = ComposeParagraph(
                context.Moon?.Summary ?? string.Empty,
                context.Ascendant?.Summary ?? string.Empty,
                analysis.EmotionalTone.Summary),
            Paragraphs = paragraphs.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = TakeHighlights(
                analysis.EmotionalTone.Keywords,
                context.Ascendant?.Keywords ?? [],
                analysis.CentralTension.Keywords)
        };
    }

    private static PremiumInterpretationBlock BuildThinkingRelatingActingBlock(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (context.Mercury is null && context.Venus is null && context.Mars is null)
        {
            return PremiumInterpretationBlock.Empty("thinkingRelatingActing", "Tu forma de pensar, vincularte y actuar");
        }

        var paragraphs = new List<string>();

        if (context.Mercury is not null)
        {
            paragraphs.Add(ComposeParagraph(
                context.Mercury.Summary,
                context.Mercury.ThinkingStyle,
                context.Mercury.CommunicationStyle,
                context.Mercury.LearningStyle));
        }

        if (context.Venus is not null)
        {
            paragraphs.Add(ComposeParagraph(
                context.Venus.Summary,
                context.Venus.RelationalStyle,
                context.Venus.AffectiveNeeds));
        }

        if (context.Mars is not null)
        {
            paragraphs.Add(ComposeParagraph(
                context.Mars.Summary,
                context.Mars.ActionStyle,
                context.Mars.DesireStyle,
                context.Mars.ConflictStyle));
        }

        return new PremiumInterpretationBlock
        {
            Key = "thinkingRelatingActing",
            Title = "Tu forma de pensar, vincularte y actuar",
            Summary = ComposeParagraph(
                analysis.RelationalStyle.Summary,
                analysis.ActionStyle.Summary,
                context.Mercury?.Summary ?? string.Empty,
                context.Venus?.Summary ?? string.Empty,
                context.Mars?.Summary ?? string.Empty),
            Paragraphs = paragraphs.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = TakeHighlights(
                context.Mercury?.Keywords ?? [],
                context.Venus?.Keywords ?? [],
                context.Mars?.Keywords ?? [])
        };
    }

    private static PremiumInterpretationBlock BuildEssentialBlock(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var paragraphs = new List<string>();

        if (!string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Summary) || !string.IsNullOrWhiteSpace(analysis.EmotionalTone.Summary))
        {
            paragraphs.Add(ComposeParagraph(
                analysis.DominantCoreTrait.Summary,
                analysis.EmotionalTone.Summary));
        }

        if (!string.IsNullOrWhiteSpace(analysis.RelationalStyle.Summary) || !string.IsNullOrWhiteSpace(analysis.ActionStyle.Summary))
        {
            paragraphs.Add(ComposeParagraph(
                analysis.RelationalStyle.Summary,
                analysis.ActionStyle.Summary));
        }

        var integrationParagraph = ComposeParagraph(
            context.Sun?.GrowthPath ?? string.Empty,
            FirstOrEmpty(context.Moon?.IntegrationHooks ?? []),
            FirstOrEmpty(context.Mars?.IntegrationHooks ?? []));

        if (!string.IsNullOrWhiteSpace(integrationParagraph))
        {
            paragraphs.Add(integrationParagraph);
        }

        var summary = ComposeParagraph(
            analysis.GrowthDirection.Headline,
            analysis.GrowthDirection.Summary,
            analysis.DominantCoreTrait.Headline,
            analysis.RelationalStyle.Headline,
            analysis.ActionStyle.Headline);

        if (string.IsNullOrWhiteSpace(summary) && paragraphs.Count == 0)
        {
            return PremiumInterpretationBlock.Empty("essential", "Lo esencial de tu carta");
        }

        return new PremiumInterpretationBlock
        {
            Key = "essential",
            Title = "Lo esencial de tu carta",
            Summary = summary,
            Paragraphs = paragraphs.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = TakeHighlights(
                analysis.DominantCoreTrait.Keywords,
                analysis.GrowthDirection.Keywords)
        };
    }

    private static string BuildClosing(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        return ComposeParagraph(
            "La fuerza de esta carta no está en elegir una sola versión de ti, sino en ordenar tus distintas capas en una dirección consciente.",
            analysis.GrowthDirection.Summary,
            context.Sun is not null ? FirstOrEmpty(context.Sun.IntegrationHooks) : string.Empty,
            analysis.DominantCoreTrait.Summary);
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
}
