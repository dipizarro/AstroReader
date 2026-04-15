namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationContentSelector
{
    private static readonly string[] EditorialTransitions =
    [
        "A la vez,",
        "En la práctica,"
    ];

    private const int HookMaxWords = 28;
    private const int EnergyCoreMainMaxWords = 36;
    private const int EnergyCoreClaimMaxWords = 48;
    private const int CoreMainMaxWords = 34;
    private const int CoreClaimMaxWords = 46;
    private const int CoreMaxSupportingClaims = 5;
    private const int PersonalDynamicsMainMaxWords = 30;
    private const int PersonalDynamicsClaimMaxWords = 58;
    private const int EssentialMainMaxWords = 28;
    private const int EssentialClaimMaxWords = 42;
    private const int EssentialMaxSupportingClaims = 4;
    private const int ClosingMaxWords = 34;

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
            Closing = SelectClosing(analysis, registry)
        };
    }

    private static string SelectHook(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        var dominantAxis = !string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline)
            ? analysis.DominantCoreTrait.Headline
            : BuildAvailableCoreLabel(context);
        var centralTension = !string.IsNullOrWhiteSpace(analysis.CentralTension.Headline)
            ? analysis.CentralTension.Headline.ToLowerInvariant()
            : string.Empty;

        if (string.IsNullOrWhiteSpace(dominantAxis))
        {
            return string.Empty;
        }

        registry.MarkUsed("dominant-core");

        var hook = !string.IsNullOrWhiteSpace(centralTension)
            ? $"{dominantAxis}, con un matiz importante: {centralTension.ToLowerInvariant()}."
            : dominantAxis;

        return UseLimited(registry, "hook.frame", hook, HookMaxWords);
    }

    private static string BuildAvailableCoreLabel(PremiumInterpretationContext context)
    {
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

        return $"La lectura parte de {string.Join(", ", availableSigns)}";
    }

    private static PremiumInterpretationBlockSelection SelectEnergyCore(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        if (context.Sun is null)
        {
            return PremiumInterpretationBlockSelection.Empty("energyCore", "Tu energía central");
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "energyCore",
            Title = "Tu energía central",
            MainClaim = UseLimited(
                registry,
                "energy-core.integrated-claim",
                BuildEnergyCoreClaim(context, analysis),
                EnergyCoreMainMaxWords),
            SupportingClaims = UseManyLimited(
                registry,
                EnergyCoreClaimMaxWords,
                ("solar-identity.summary", context.Sun.Summary),
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

        var mainClaim = UseLimited(
            registry,
            "core.development-frame",
            BuildCoreFrame(context),
            CoreMainMaxWords);

        var supportingClaims = new List<string>();

        if (context.Moon is not null)
        {
            supportingClaims.AddRange(UseManyLimited(
                registry,
                CoreClaimMaxWords,
                ("emotional-style", context.Moon.EmotionalStyle),
                ("emotional-needs", context.Moon.EmotionalNeeds),
                ("security-needs", context.Moon.SecurityNeeds)));
        }

        if (context.Ascendant is not null)
        {
            supportingClaims.AddRange(UseManyLimited(
                registry,
                CoreClaimMaxWords,
                ("outer-presence", context.Ascendant.OuterStyle),
                ("social-presence", context.Ascendant.SocialStyle),
                ("first-impression", context.Ascendant.FirstImpression)));
        }

        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            supportingClaims.Add(UseLimited(
                registry,
                "central-tension",
                BuildTensionDevelopmentClaim(analysis),
                CoreClaimMaxWords));
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "core",
            Title = "Tu núcleo",
            MainClaim = mainClaim,
            SupportingClaims = supportingClaims
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(CoreMaxSupportingClaims)
                .ToList(),
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

        var mainClaim = UseLimited(
            registry,
            "personal-dynamics.frame",
            BuildPersonalDynamicsFrame(context),
            PersonalDynamicsMainMaxWords);

        var supportingClaims = new List<string>();

        if (context.Mercury is not null)
        {
            supportingClaims.Add(UseLimited(
                registry,
                "mental-style",
                ComposeParagraph(
                    context.Mercury.ThinkingStyle,
                    context.Mercury.CommunicationStyle,
                    context.Mercury.LearningStyle),
                PersonalDynamicsClaimMaxWords));
        }

        if (context.Venus is not null)
        {
            supportingClaims.Add(UseLimited(
                registry,
                "relational-style",
                ComposeParagraph(
                    context.Venus.RelationalStyle,
                    context.Venus.AttractionStyle,
                    context.Venus.AffectiveNeeds),
                PersonalDynamicsClaimMaxWords));
        }

        if (context.Mars is not null)
        {
            supportingClaims.Add(UseLimited(
                registry,
                "action-style",
                ComposeParagraph(
                    context.Mars.ActionStyle,
                    context.Mars.DesireStyle,
                    context.Mars.ConflictStyle),
                PersonalDynamicsClaimMaxWords));
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

        var centralPattern = BuildEssentialCentralClaim(context, analysis);
        if (!string.IsNullOrWhiteSpace(centralPattern))
        {
            supportingClaims.Add(UseLimited(
                registry,
                "essential.central-pattern",
                centralPattern,
                EssentialClaimMaxWords));
        }

        var tensionClaim = BuildEssentialTensionClaim(analysis);
        if (!string.IsNullOrWhiteSpace(tensionClaim))
        {
            supportingClaims.Add(UseLimited(
                registry,
                "essential.tension",
                tensionClaim,
                EssentialClaimMaxWords));
        }

        var dynamicsClaim = BuildDynamicsClaim(analysis);
        if (!string.IsNullOrWhiteSpace(dynamicsClaim))
        {
            supportingClaims.Add(UseLimited(
                registry,
                "personal-dynamics.synthesis",
                dynamicsClaim,
                EssentialClaimMaxWords));
        }

        var growthClaim = BuildGrowthClaim(context, analysis);
        if (!string.IsNullOrWhiteSpace(growthClaim))
        {
            supportingClaims.Add(UseLimited(
                registry,
                "growth-direction",
                growthClaim,
                EssentialClaimMaxWords));
        }

        var filteredClaims = supportingClaims
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Take(EssentialMaxSupportingClaims)
            .ToList();

        if (filteredClaims.Count == 0)
        {
            return PremiumInterpretationBlockSelection.Empty("essential", "Lo esencial de tu carta");
        }

        return new PremiumInterpretationBlockSelection
        {
            Key = "essential",
            Title = "Lo esencial de tu carta",
            MainClaim = UseLimited(
                registry,
                "essential.frame",
                BuildEssentialFrame(analysis),
                EssentialMainMaxWords),
            SupportingClaims = filteredClaims,
            Highlights = TakeHighlights(
                analysis.DominantCoreTrait.Keywords,
                analysis.GrowthDirection.Keywords)
        };
    }

    private static string SelectClosing(
        InterpretationAnalysisResult analysis,
        PremiumInterpretationClaimRegistry registry)
    {
        return UseLimited(
            registry,
            "closing.coda",
            BuildClosingCoda(analysis),
            ClosingMaxWords);
    }

    private static string BuildClosingCoda(InterpretationAnalysisResult analysis)
    {
        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            return "El cierre está en sostener la complejidad sin resolverla a la fuerza: ahí la lectura se vuelve dirección, no etiqueta.";
        }

        if (!string.IsNullOrWhiteSpace(analysis.GrowthDirection.Headline))
        {
            return "El valor de esta lectura aparece cuando deja de ser descripción y se convierte en una forma más consciente de elegir.";
        }

        return "Quédate con esto: la carta no te reduce, te ofrece un mapa para habitarte con más intención.";
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
            1 => $"Esta sección desarrolla principalmente tu dinámica de {availableParts[0]}, sin mezclarla con el resto del núcleo.",
            2 => $"Esta sección une tu dinámica de {availableParts[0]} y {availableParts[1]} para mostrar cómo se mueve tu vida práctica.",
            _ => "Esta sección integra cómo piensas, cómo te vinculas y cómo movilizas deseo y acción."
        };
    }

    private static string BuildEnergyCoreClaim(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var parts = new List<string>();

        if (context.Sun is not null)
        {
            parts.Add($"tu identidad se organiza desde {context.SunSign}");
        }

        if (context.Moon is not null)
        {
            parts.Add($"tu mundo emocional responde desde {context.MoonSign}");
        }

        if (context.Ascendant is not null)
        {
            parts.Add($"tu presencia entra al mundo desde {context.AscendantSign}");
        }

        if (parts.Count == 0)
        {
            return string.Empty;
        }

        var frame = $"Tu energía central combina {JoinNatural(parts)}.";

        return !string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline)
            ? $"{frame} Esa mezcla define el tono de base sin agotar toda la lectura."
            : frame;
    }

    private static string BuildCoreFrame(PremiumInterpretationContext context)
    {
        if (context.Moon is not null && context.Ascendant is not null)
        {
            return "Este bloque baja del eje general al modo en que sientes, te regulas y eres percibido al entrar en contacto.";
        }

        if (context.Moon is not null)
        {
            return "Este bloque desarrolla tu mundo emocional y la forma en que buscas seguridad interna.";
        }

        return "Este bloque desarrolla tu presencia externa y la forma en que otros suelen recibir tu energía inicial.";
    }

    private static string BuildTensionDevelopmentClaim(InterpretationAnalysisResult analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            return string.Empty;
        }

        return $"La tensión a observar no es un defecto, sino un punto de ajuste: {analysis.CentralTension.Headline.ToLowerInvariant()}.";
    }

    private static string BuildEssentialCentralClaim(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.DominantCoreTrait.Headline))
        {
            var availableCore = BuildAvailableCoreLabel(context);

            return !string.IsNullOrWhiteSpace(availableCore)
                ? "La idea central es leer las piezas disponibles como un sistema, no como rasgos aislados."
                : string.Empty;
        }

        return "La idea central no está en acumular rasgos, sino en reconocer el patrón que organiza tu identidad, tu emoción y tu forma de presentarte.";
    }

    private static string BuildEssentialTensionClaim(InterpretationAnalysisResult analysis)
    {
        if (string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            return string.Empty;
        }

        return "La tensión principal funciona como un punto de afinación: no invalida tu carta, pero muestra dónde conviene ganar conciencia y elección.";
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
            1 => $"En lo cotidiano, la dinámica más visible aparece en tu {headlines[0]}.",
            _ => $"En lo cotidiano, tu carta se mueve especialmente entre tu {headlines[0]} y tu {headlines[1]}."
        };
    }

    private static string BuildEssentialFrame(InterpretationAnalysisResult analysis)
    {
        if (!string.IsNullOrWhiteSpace(analysis.CentralTension.Headline))
        {
            return "Lo esencial es mirar el patrón central, la tensión que lo afina y una dirección concreta de integración.";
        }

        return "Lo esencial es mirar el patrón central y convertirlo en una dirección concreta de integración.";
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

        if (normalized.Count == 0)
        {
            return string.Empty;
        }

        if (normalized.Count == 1)
        {
            return EnsureTerminalPunctuation(normalized[0]);
        }

        if (normalized[0].EndsWith(':'))
        {
            return ComposeAfterLeadIn(normalized);
        }

        var sentences = new List<string>
        {
            EnsureTerminalPunctuation(normalized[0])
        };

        for (var index = 1; index < normalized.Count; index++)
        {
            var transition = EditorialTransitions[(index - 1) % EditorialTransitions.Length];
            var fragment = LowercaseInitial(normalized[index]);

            sentences.Add(EnsureTerminalPunctuation($"{transition} {fragment}"));
        }

        return string.Join(" ", sentences);
    }

    private static string ComposeAfterLeadIn(IReadOnlyList<string> fragments)
    {
        var leadIn = fragments[0];
        var remaining = fragments
            .Skip(1)
            .Select(EnsureTerminalPunctuation)
            .ToList();

        return remaining.Count == 0
            ? leadIn
            : $"{leadIn} {string.Join(" ", remaining)}";
    }

    private static string UseLimited(
        PremiumInterpretationClaimRegistry registry,
        string claimKey,
        string value,
        int maxWords)
    {
        return registry.Use(claimKey, LimitWords(value, maxWords));
    }

    private static IReadOnlyList<string> UseManyLimited(
        PremiumInterpretationClaimRegistry registry,
        int maxWords,
        params (string ClaimKey, string Value)[] claims)
    {
        return claims
            .Select(claim => UseLimited(registry, claim.ClaimKey, claim.Value, maxWords))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static string JoinNatural(IReadOnlyList<string> parts)
    {
        return parts.Count switch
        {
            0 => string.Empty,
            1 => parts[0],
            2 => $"{parts[0]} y {parts[1]}",
            _ => $"{string.Join(", ", parts.Take(parts.Count - 1))} y {parts[^1]}"
        };
    }

    private static string LimitWords(string value, int maxWords)
    {
        var normalized = NormalizeText(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return words.Length <= maxWords
            ? normalized
            : $"{string.Join(" ", words.Take(maxWords))}.";
    }

    private static string EnsureTerminalPunctuation(string value)
    {
        var normalized = NormalizeText(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        return normalized[^1] is '.' or '!' or '?' or ':'
            ? normalized
            : $"{normalized}.";
    }

    private static string LowercaseInitial(string value)
    {
        var normalized = NormalizeText(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        return normalized.Length == 1
            ? normalized.ToLowerInvariant()
            : $"{char.ToLowerInvariant(normalized[0])}{normalized[1..]}";
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

    private static string NormalizeText(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }

}
