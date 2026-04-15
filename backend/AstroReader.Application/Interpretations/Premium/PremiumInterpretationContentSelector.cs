namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationContentSelector
{
    public static PremiumInterpretationContentSelectionPlan CreatePlan(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var registry = new PremiumInterpretationClaimRegistry();

        return new PremiumInterpretationContentSelectionPlan
        {
            Hook = SelectHook(context, analysis, registry),
            EnergyCore = SelectEnergyCore(context, analysis, registry),
            Core = SelectCore(context, analysis, registry),
            PersonalDynamics = SelectPersonalDynamics(context, analysis, registry),
            EssentialSummary = SelectEssentialSummary(context, analysis, registry),
            Closing = SelectClosing(context, analysis, registry)
        };
    }

    private static string SelectHook(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        if (!string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline))
        {
            return registry.Use("dominant-core", analysis.DominantCoreTrait.Headline);
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

        return registry.Use(
            "available-core",
            $"Esta lectura integra {string.Join(", ", availableSigns)} como base principal de la carta.");
    }

    private static PremiumInterpretationBlockSelection SelectEnergyCore(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        if (context.Sun is null)
        {
            return PremiumInterpretationBlockSelection.Empty("centralEnergy", "Tu energía central");
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "centralEnergy",
            Title = "Tu energía central",
            MainClaim = registry.Use("solar-identity.summary", context.Sun.Summary),
            SupportingClaims = registry.UseMany(
                ("solar-identity.style", context.Sun.IdentityStyle)),
            Highlights = analysis.DominantCoreTrait.Keywords.Count > 0
                ? analysis.DominantCoreTrait.Keywords
                : context.Sun.Keywords
        };
    }

    private static PremiumInterpretationBlockSelection SelectCore(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        if (context.Moon is null && context.Ascendant is null)
        {
            return PremiumInterpretationBlockSelection.Empty("core", "Tu núcleo");
        }

        var mainClaim = registry.Use(
            "emotional-presence.summary",
            ComposeParagraph(
                context.Moon?.Summary ?? string.Empty,
                context.Ascendant?.Summary ?? string.Empty));

        var supportingClaims = new List<string>();

        if (context.Moon is not null)
        {
            supportingClaims.AddRange(registry.UseMany(
                ("emotional-style", context.Moon.EmotionalStyle),
                ("emotional-needs", context.Moon.EmotionalNeeds),
                ("security-needs", context.Moon.SecurityNeeds)));
        }

        if (context.Ascendant is not null)
        {
            supportingClaims.AddRange(registry.UseMany(
                ("outer-presence", context.Ascendant.OuterStyle),
                ("social-presence", context.Ascendant.SocialStyle),
                ("first-impression", context.Ascendant.FirstImpression)));
        }

        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            supportingClaims.Add(registry.Use(
                "central-tension",
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
        PremiumInterpretationClaimRegistry registry)
    {
        if (context.Mercury is null && context.Venus is null && context.Mars is null)
        {
            return PremiumInterpretationBlockSelection.Empty("thinkingRelatingActing", "Tu forma de pensar, vincularte y actuar");
        }

        var mainClaim = registry.Use(
            "personal-dynamics.frame",
            BuildPersonalDynamicsFrame(context));

        var supportingClaims = new List<string>();

        if (context.Mercury is not null)
        {
            supportingClaims.Add(registry.Use(
                "mental-style",
                ComposeParagraph(
                    context.Mercury.ThinkingStyle,
                    context.Mercury.CommunicationStyle,
                    context.Mercury.LearningStyle)));
        }

        if (context.Venus is not null)
        {
            supportingClaims.Add(registry.Use(
                "relational-style",
                ComposeParagraph(
                    context.Venus.RelationalStyle,
                    context.Venus.AttractionStyle,
                    context.Venus.AffectiveNeeds)));
        }

        if (context.Mars is not null)
        {
            supportingClaims.Add(registry.Use(
                "action-style",
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
        PremiumInterpretationClaimRegistry registry)
    {
        var supportingClaims = new List<string>();

        var centralPattern = BuildCentralPatternClaim(analysis);
        if (!string.IsNullOrWhiteSpace(centralPattern))
        {
            supportingClaims.Add(registry.Use("dominant-core", centralPattern));
        }

        var tensionClaim = BuildTensionClaim(analysis);
        if (!string.IsNullOrWhiteSpace(tensionClaim))
        {
            supportingClaims.Add(registry.Use("central-tension", tensionClaim));
        }

        var dynamicsClaim = BuildDynamicsClaim(analysis);
        if (!string.IsNullOrWhiteSpace(dynamicsClaim))
        {
            supportingClaims.Add(registry.Use("personal-dynamics.synthesis", dynamicsClaim));
        }

        var growthClaim = BuildGrowthClaim(context, analysis);
        if (!string.IsNullOrWhiteSpace(growthClaim))
        {
            supportingClaims.Add(registry.Use("growth-direction", growthClaim));
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
            MainClaim = registry.Use(
                "essential.frame",
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
        PremiumInterpretationClaimRegistry registry)
    {
        var closingHook = FirstOrEmpty(context.Sun?.IntegrationHooks ?? []);

        if (!string.IsNullOrWhiteSpace(closingHook))
        {
            return registry.UseOrFallback(
                "growth-direction",
                ComposeParagraph(
                    "Como cierre práctico:",
                    closingHook),
                "Como cierre, elige una práctica pequeña que vuelva esta lectura visible en tu vida diaria.");
        }

        if (!string.IsNullOrWhiteSpace(analysis.GrowthDirection.Headline))
        {
            return registry.UseOrFallback(
                "growth-direction",
                "La integración de esta carta empieza cuando conviertes autoconocimiento en una decisión pequeña y concreta.",
                "Como cierre, quédate con una decisión pequeña y concreta que puedas practicar en lo cotidiano.");
        }

        return registry.Use(
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

}
