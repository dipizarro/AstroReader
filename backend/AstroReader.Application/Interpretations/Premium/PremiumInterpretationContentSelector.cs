namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationContentSelector
{
    public static PremiumInterpretationContentSelectionPlan CreatePlan(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var draft = new ContentDraft();

        return new PremiumInterpretationContentSelectionPlan
        {
            Hook = SelectHook(context, analysis, draft),
            EnergyCore = SelectEnergyCore(context, analysis, draft),
            Core = SelectCore(context, analysis, draft),
            PersonalDynamics = SelectPersonalDynamics(context, analysis, draft),
            EssentialSummary = SelectEssentialSummary(context, analysis, draft),
            Closing = SelectClosing(context, analysis, draft)
        };
    }

    private static string SelectHook(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        if (!string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline))
        {
            return draft.Use("hook.dominantCoreTrait", analysis.DominantCoreTrait.Headline);
        }

        var availableSigns = new List<string>();

        if (context.Sun is not null)
        {
            availableSigns.Add($"Sol en {context.SunSign}");
        }

        if (context.Moon is not null)
        {
            availableSigns.Add($"Luna en {context.MoonSign}");
        }

        if (context.Ascendant is not null)
        {
            availableSigns.Add($"Ascendente en {context.AscendantSign}");
        }

        if (availableSigns.Count == 0)
        {
            return string.Empty;
        }

        return draft.Use(
            "hook.availableCore",
            $"Esta lectura integra {string.Join(", ", availableSigns)} como base principal de la carta.");
    }

    private static PremiumInterpretationBlockSelection SelectEnergyCore(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        if (context.Sun is null)
        {
            return PremiumInterpretationBlockSelection.Empty("centralEnergy", "Tu energía central");
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "centralEnergy",
            Title = "Tu energía central",
            MainClaim = draft.Use("sun.summary", context.Sun.Summary),
            SupportingClaims = draft.UseMany(
                ("sun.identityStyle", context.Sun.IdentityStyle)),
            Highlights = analysis.DominantCoreTrait.Keywords.Count > 0
                ? analysis.DominantCoreTrait.Keywords
                : context.Sun.Keywords
        };
    }

    private static PremiumInterpretationBlockSelection SelectCore(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        if (context.Moon is null && context.Ascendant is null)
        {
            return PremiumInterpretationBlockSelection.Empty("core", "Tu núcleo");
        }

        var mainClaim = draft.Use(
            "core.summary",
            ComposeParagraph(
                context.Moon?.Summary ?? string.Empty,
                context.Ascendant?.Summary ?? string.Empty));

        var supportingClaims = new List<string>();

        if (context.Moon is not null)
        {
            supportingClaims.AddRange(draft.UseMany(
                ("moon.emotionalStyle", context.Moon.EmotionalStyle),
                ("moon.emotionalNeeds", context.Moon.EmotionalNeeds),
                ("moon.securityNeeds", context.Moon.SecurityNeeds)));
        }

        if (context.Ascendant is not null)
        {
            supportingClaims.AddRange(draft.UseMany(
                ("ascendant.outerStyle", context.Ascendant.OuterStyle),
                ("ascendant.socialStyle", context.Ascendant.SocialStyle),
                ("ascendant.firstImpression", context.Ascendant.FirstImpression)));
        }

        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            supportingClaims.Add(draft.Use(
                "analysis.centralTension.headline",
                analysis.CentralTension.Headline));
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "core",
            Title = "Tu núcleo",
            MainClaim = mainClaim,
            SupportingClaims = supportingClaims.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = TakeHighlights(
                analysis.EmotionalTone.Keywords,
                context.Ascendant?.Keywords ?? [],
                analysis.CentralTension.Keywords)
        };
    }

    private static PremiumInterpretationBlockSelection SelectPersonalDynamics(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        if (context.Mercury is null && context.Venus is null && context.Mars is null)
        {
            return PremiumInterpretationBlockSelection.Empty("thinkingRelatingActing", "Tu forma de pensar, vincularte y actuar");
        }

        var mainClaim = draft.Use(
            "personalDynamics.frame",
            BuildPersonalDynamicsFrame(context));

        var supportingClaims = new List<string>();

        if (context.Mercury is not null)
        {
            supportingClaims.Add(draft.Use(
                "mercury.claim",
                ComposeParagraph(
                    context.Mercury.ThinkingStyle,
                    context.Mercury.CommunicationStyle,
                    context.Mercury.LearningStyle)));
        }

        if (context.Venus is not null)
        {
            supportingClaims.Add(draft.Use(
                "venus.claim",
                ComposeParagraph(
                    context.Venus.RelationalStyle,
                    context.Venus.AttractionStyle,
                    context.Venus.AffectiveNeeds)));
        }

        if (context.Mars is not null)
        {
            supportingClaims.Add(draft.Use(
                "mars.claim",
                ComposeParagraph(
                    context.Mars.ActionStyle,
                    context.Mars.DesireStyle,
                    context.Mars.ConflictStyle)));
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "thinkingRelatingActing",
            Title = "Tu forma de pensar, vincularte y actuar",
            MainClaim = mainClaim,
            SupportingClaims = supportingClaims.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
            Highlights = TakeHighlights(
                context.Mercury?.Keywords ?? [],
                context.Venus?.Keywords ?? [],
                context.Mars?.Keywords ?? [])
        };
    }

    private static PremiumInterpretationBlockSelection SelectEssentialSummary(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        var supportingClaims = new List<string>();

        var centralPattern = BuildCentralPatternClaim(analysis);
        if (!string.IsNullOrWhiteSpace(centralPattern))
        {
            supportingClaims.Add(draft.Use("essential.centralPattern", centralPattern));
        }

        var tensionClaim = BuildTensionClaim(analysis);
        if (!string.IsNullOrWhiteSpace(tensionClaim))
        {
            supportingClaims.Add(draft.Use("essential.tension", tensionClaim));
        }

        var dynamicsClaim = BuildDynamicsClaim(analysis);
        if (!string.IsNullOrWhiteSpace(dynamicsClaim))
        {
            supportingClaims.Add(draft.Use("essential.dynamics", dynamicsClaim));
        }

        var growthClaim = BuildGrowthClaim(context, analysis);
        if (!string.IsNullOrWhiteSpace(growthClaim))
        {
            supportingClaims.Add(draft.Use("essential.growth", growthClaim));
        }

        var filteredClaims = supportingClaims
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (filteredClaims.Count == 0)
        {
            return PremiumInterpretationBlockSelection.Empty("essential", "Lo esencial de tu carta");
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "essential",
            Title = "Lo esencial de tu carta",
            MainClaim = draft.Use(
                "essential.summary",
                "La lectura se ordena mejor cuando miras estas piezas como un sistema y no como rasgos aislados."),
            SupportingClaims = filteredClaims,
            Highlights = TakeHighlights(
                analysis.DominantCoreTrait.Keywords,
                analysis.GrowthDirection.Keywords)
        };
    }

    private static string SelectClosing(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        ContentDraft draft)
    {
        var closingHook = FirstOrEmpty(context.Sun?.IntegrationHooks ?? []);

        if (!string.IsNullOrWhiteSpace(closingHook))
        {
            return draft.Use(
                "closing.sunIntegrationHook",
                ComposeParagraph(
                    "Como cierre práctico:",
                    closingHook));
        }

        if (!string.IsNullOrWhiteSpace(analysis.GrowthDirection.Headline))
        {
            return draft.Use(
                "closing.growthDirection",
                "La integración de esta carta empieza cuando conviertes autoconocimiento en una decisión pequeña y concreta.");
        }

        return draft.Use(
            "closing.default",
            "La lectura gana fuerza cuando puedes observar estas capas sin reducirte a una sola de ellas.");
    }

    private static string BuildPersonalDynamicsFrame(PremiumInterpretationContext context)
    {
        var availableParts = new List<string>();

        if (context.Mercury is not null)
        {
            availableParts.Add("mente");
        }

        if (context.Venus is not null)
        {
            availableParts.Add("vínculo");
        }

        if (context.Mars is not null)
        {
            availableParts.Add("acción");
        }

        return availableParts.Count switch
        {
            0 => string.Empty,
            1 => $"Esta sección muestra principalmente tu dinámica de {availableParts[0]}.",
            2 => $"Esta sección integra tu dinámica de {availableParts[0]} y {availableParts[1]}.",
            _ => "Esta sección integra cómo piensas, cómo te vinculas y cómo movilizas deseo y acción."
        };
    }

    private static string BuildCentralPatternClaim(InterpretationAnalysisResult analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline))
        {
            return string.Empty;
        }

        return $"El patrón dominante aparece como {analysis.DominantCoreTrait.Headline.ToLowerInvariant()}.";
    }

    private static string BuildTensionClaim(InterpretationAnalysisResult analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            return string.Empty;
        }

        return $"El punto de contraste a observar es la {analysis.CentralTension.Headline.ToLowerInvariant()}.";
    }

    private static string BuildDynamicsClaim(InterpretationAnalysisResult analysis)
    {
        var headlines = new List<string>();

        if (!string.IsNullOrWhiteSpace(analysis.RelationalStyle.Headline))
        {
            headlines.Add(analysis.RelationalStyle.Headline.ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(analysis.ActionStyle.Headline))
        {
            headlines.Add(analysis.ActionStyle.Headline.ToLowerInvariant());
        }

        return headlines.Count switch
        {
            0 => string.Empty,
            1 => $"La dinámica práctica más visible está en tu {headlines[0]}.",
            _ => $"La dinámica práctica de la carta se expresa en tu {headlines[0]} y en tu {headlines[1]}."
        };
    }

    private static string BuildGrowthClaim(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (!string.IsNullOrWhiteSpace(context.Sun?.GrowthPath))
        {
            return context.Sun!.GrowthPath;
        }

        return !string.IsNullOrWhiteSpace(analysis.GrowthDirection.Summary)
            ? analysis.GrowthDirection.Summary
            : string.Empty;
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

    private sealed class ContentDraft
    {
        private readonly HashSet<string> _usedKeys = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _usedTexts = new(StringComparer.OrdinalIgnoreCase);

        public string Use(string key, string value)
        {
            var normalized = NormalizeText(value);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return string.Empty;
            }

            if (!_usedKeys.Add(key) || !_usedTexts.Add(normalized))
            {
                return string.Empty;
            }

            return normalized;
        }

        public IReadOnlyList<string> UseMany(params (string Key, string Value)[] claims)
        {
            return claims
                .Select(claim => Use(claim.Key, claim.Value))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
