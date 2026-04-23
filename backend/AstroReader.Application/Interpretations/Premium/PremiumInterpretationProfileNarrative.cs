using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationProfileNarrative
{
    private sealed record EditorialTheme(string Key, string Label, params string[] Cues);

    private sealed record InsightMatch(
        string InsightKey,
        string InsightLabel,
        string EditorialFrame,
        int Score,
        IReadOnlyList<EditorialTheme> Themes);

    private static readonly IReadOnlyList<EditorialTheme> EditorialThemes =
    [
        new("sensitivity", "sensibilidad y registro fino", "sensibilidad", "sensible", "emocion", "emocional", "intuicion", "intuit", "vulnerab", "afecto", "afect", "cuidado", "ternura"),
        new("clarity", "claridad y comprensión", "claridad", "confusion", "entender", "comprender", "pensar", "mente", "perspectiva", "orden", "enfoque"),
        new("relationship", "vínculo y comunicación", "vinculo", "relacion", "pareja", "amor", "otros", "comunica", "expresar", "limite", "cercania"),
        new("action", "acción y dirección", "accion", "decidir", "decision", "hacer", "avance", "mover", "impulso", "energia", "iniciativa"),
        new("security", "seguridad interna y regulación", "seguridad", "calma", "ansiedad", "miedo", "control", "confianza", "estabilidad", "refugio"),
        new("identity", "identidad y autenticidad", "identidad", "autentic", "proposito", "direccion", "sentido", "brillo", "expresion", "valor"),
        new("intensity", "intensidad y transformación", "intensidad", "cambio", "transform", "crisis", "profund", "extremo", "todo")
    ];

    public static string ApplyHookPersonalization(string hook, PremiumReaderProfileContext? profile)
    {
        if (string.IsNullOrWhiteSpace(hook) || string.IsNullOrWhiteSpace(profile?.DisplayName))
        {
            return hook;
        }

        return $"{profile.DisplayName}, {LowercaseInitial(hook)}";
    }

    public static string ApplyClosingPersonalization(string closing, PremiumReaderProfileContext? profile)
    {
        if (profile is null || !profile.HasEditorialContext)
        {
            return closing;
        }

        var contextualTail = !string.IsNullOrWhiteSpace(profile.DesiredInsight)
            ? $"Hoy esta lectura puede servirte especialmente para mirar con más claridad {EnsureTerminalPunctuation(LowercaseInitial(profile.DesiredInsight))}"
            : !string.IsNullOrWhiteSpace(profile.CurrentChallenge)
                ? $"Vuelve a esta lectura cuando necesites tomar perspectiva sobre {EnsureTerminalPunctuation(LowercaseInitial(profile.CurrentChallenge))}"
                : string.Empty;

        if (string.IsNullOrWhiteSpace(contextualTail))
        {
            return closing;
        }

        return string.IsNullOrWhiteSpace(closing)
            ? contextualTail
            : $"{closing} {contextualTail}";
    }

    public static IReadOnlyList<PremiumInterpretationProfile> BuildProfiles(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        if (context.ReaderProfile is null || !context.ReaderProfile.HasEditorialContext)
        {
            return [];
        }

        var profiles = new List<PremiumInterpretationProfile>
        {
            BuildContrastProfile(context, analysis)
        };

        var priorityProfile = BuildPriorityProfile(context);
        if (priorityProfile is not null)
        {
            profiles.Add(priorityProfile);
        }

        return profiles;
    }

    public static IReadOnlyList<PremiumInterpretationProfile> BuildFallbackProfiles(
        PremiumReaderProfileContext? profile,
        Domain.Enums.ZodiacSign sunSign,
        Domain.Enums.ZodiacSign moonSign,
        Domain.Enums.ZodiacSign ascendantSign)
    {
        if (profile is null || !profile.HasEditorialContext)
        {
            return [];
        }

        var profiles = new List<PremiumInterpretationProfile>
        {
            BuildFallbackContrastProfile(profile, sunSign, moonSign, ascendantSign)
        };

        var priorityProfile = BuildFallbackPriorityProfile(profile);
        if (priorityProfile is not null)
        {
            profiles.Add(priorityProfile);
        }

        return profiles;
    }

    private static PremiumInterpretationProfile BuildContrastProfile(
        PremiumInterpretationContext context,
        InterpretationAnalysisResult analysis)
    {
        var profile = context.ReaderProfile!;
        var selfPerceptionThemes = ScoreThemes(profile.SelfPerceptionFocus, profile.SelfDescription);
        var challengeThemes = ScoreThemes(profile.CurrentChallenge);
        var desiredInsightThemes = ScoreThemes(profile.DesiredInsight);

        var resonanceMatch = FindBestResonanceMatch(analysis, selfPerceptionThemes);
        var contrastMatch = FindBestContrastMatch(analysis, challengeThemes, desiredInsightThemes, resonanceMatch);

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(profile.DisplayName))
        {
            parts.Add($"{profile.DisplayName}, tu carta sigue marcando el tono de fondo desde Sol en {context.SunSign}, Luna en {context.MoonSign} y Ascendente en {context.AscendantSign}.");
        }
        else
        {
            parts.Add($"Tu carta sigue marcando el tono de fondo desde Sol en {context.SunSign}, Luna en {context.MoonSign} y Ascendente en {context.AscendantSign}.");
        }

        if (!string.IsNullOrWhiteSpace(profile.SelfPerceptionFocus))
        {
            parts.Add($"Hoy te reconoces especialmente en {EnsureTerminalPunctuation(LowercaseInitial(profile.SelfPerceptionFocus))}");
        }

        if (resonanceMatch is not null)
        {
            parts.Add(
                $"Ahí aparece una coincidencia valiosa con la carta: {resonanceMatch.EditorialFrame} " +
                $"No es solo una impresión momentánea, sino una línea que también se deja leer en su estructura.");
        }

        if (!string.IsNullOrWhiteSpace(profile.CurrentChallenge) && contrastMatch is not null)
        {
            parts.Add(
                $"También hay un contraste fértil. Lo que hoy nombras como {EnsureTerminalPunctuation(LowercaseInitial(profile.CurrentChallenge))} " +
                $"la carta lo reubica en el plano de {contrastMatch.InsightLabel.ToLowerInvariant()}. " +
                $"Más que desmentirte, eso añade profundidad y perspectiva.");
        }
        else if (!string.IsNullOrWhiteSpace(profile.CurrentChallenge))
        {
            parts.Add($"Ese foco convive con un desafío claro: {EnsureTerminalPunctuation(LowercaseInitial(profile.CurrentChallenge))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.DesiredInsight))
        {
            parts.Add(
                $"Por eso esta lectura puede ayudarte a mirar {EnsureTerminalPunctuation(LowercaseInitial(profile.DesiredInsight))} " +
                $"sin reducirlo a una sola etiqueta.");
        }

        if (!string.IsNullOrWhiteSpace(profile.SelfDescription))
        {
            parts.Add($"Tu propia descripción añade una nota más íntima a esta lectura: {TrimAndEnsureTerminal(profile.SelfDescription!, 110)}");
        }

        return new PremiumInterpretationProfile
        {
            Key = "self-perception-contrast",
            Title = "Tu carta y cómo hoy te percibes",
            Summary = string.Join(" ", parts)
        };
    }

    private static PremiumInterpretationProfile? BuildPriorityProfile(PremiumInterpretationContext context)
    {
        var profile = context.ReaderProfile!;
        var focus = !string.IsNullOrWhiteSpace(profile.DesiredInsight)
            ? profile.DesiredInsight
            : profile.CurrentChallenge;

        if (string.IsNullOrWhiteSpace(focus))
        {
            return null;
        }

        var priority = ResolvePriority(focus);

        return new PremiumInterpretationProfile
        {
            Key = "reading-priority",
            Title = "Dónde puede ayudarte más esta lectura hoy",
            Summary =
                $"Sin mover el centro de la carta, hoy conviene entrar primero por \"{priority.BlockTitle}\". {priority.Reason} " +
                $"Ese puede ser el mejor punto de apoyo para mirar {EnsureTerminalPunctuation(LowercaseInitial(focus))}"
        };
    }

    private static PremiumInterpretationProfile BuildFallbackContrastProfile(
        PremiumReaderProfileContext profile,
        Domain.Enums.ZodiacSign sunSign,
        Domain.Enums.ZodiacSign moonSign,
        Domain.Enums.ZodiacSign ascendantSign)
    {
        var parts = new List<string>
        {
            $"Aunque esta salida siga siendo una lectura base, tu carta ya marca un tono de fondo desde Sol en {sunSign}, Luna en {moonSign} y Ascendente en {ascendantSign}."
        };

        if (!string.IsNullOrWhiteSpace(profile.SelfPerceptionFocus))
        {
            parts.Add($"Hoy te reconoces especialmente en {EnsureTerminalPunctuation(LowercaseInitial(profile.SelfPerceptionFocus))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.CurrentChallenge))
        {
            parts.Add(
                $"Eso convive con algo que hoy pesa de forma concreta: {EnsureTerminalPunctuation(LowercaseInitial(profile.CurrentChallenge))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.DesiredInsight))
        {
            parts.Add(
                $"Incluso desde esta versión base, la lectura puede ayudarte a mirar con más perspectiva {EnsureTerminalPunctuation(LowercaseInitial(profile.DesiredInsight))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.SelfDescription))
        {
            parts.Add($"Tu propia descripción añade una nota cercana a esta lectura: {TrimAndEnsureTerminal(profile.SelfDescription!, 110)}");
        }

        return new PremiumInterpretationProfile
        {
            Key = "self-perception-contrast",
            Title = "Tu carta y cómo hoy te percibes",
            Summary = string.Join(" ", parts)
        };
    }

    private static PremiumInterpretationProfile? BuildFallbackPriorityProfile(PremiumReaderProfileContext profile)
    {
        var focus = !string.IsNullOrWhiteSpace(profile.DesiredInsight)
            ? profile.DesiredInsight
            : profile.CurrentChallenge;

        if (string.IsNullOrWhiteSpace(focus))
        {
            return null;
        }

        var priority = ResolvePriority(focus);

        return new PremiumInterpretationProfile
        {
            Key = "reading-priority",
            Title = "Dónde puede ayudarte más esta lectura hoy",
            Summary =
                $"Como esta respuesta todavía no desarrolla toda la capa premium, conviene entrar primero por \"{priority.BlockTitle}\". {priority.Reason} " +
                $"Es la forma más útil de empezar a mirar {EnsureTerminalPunctuation(LowercaseInitial(focus))} con lo que esta versión ya puede sostener."
        };
    }

    private static (string BlockTitle, string Reason) ResolvePriority(string focus)
    {
        var normalized = focus.ToLowerInvariant();

        if (ContainsAny(normalized, "amor", "pareja", "vinc", "relaci", "comunic", "límite", "limite"))
        {
            return (
                "Tu forma de pensar, vincularte y actuar",
                "Lo que hoy buscas entender parece jugarse sobre todo en tu manera de relacionarte, comunicarte y movilizar decisiones."
            );
        }

        if (ContainsAny(normalized, "emoci", "calma", "seguridad", "ansiedad", "sensibilidad", "miedo", "herida"))
        {
            return (
                "Tu núcleo",
                "Lo que hoy se mueve con más fuerza parece necesitar una lectura emocional y de autorregulación antes que una explicación abstracta."
            );
        }

        if (ContainsAny(normalized, "identidad", "propósito", "proposito", "dirección", "direccion", "autentic", "brillo"))
        {
            return (
                "Tu energía central",
                "El foco actual parece tocar directamente tu eje de identidad, vitalidad y sentido personal."
            );
        }

        return (
            "Lo esencial de tu carta",
            "En este caso ayuda más empezar por la síntesis general y luego bajar al detalle desde allí."
        );
    }

    private static InsightMatch? FindBestResonanceMatch(
        InterpretationAnalysisResult analysis,
        IReadOnlyDictionary<string, int> profileThemes)
    {
        if (profileThemes.Count == 0)
        {
            return null;
        }

        var candidates = new[]
        {
            BuildInsightMatch(analysis.DominantCoreTrait, "núcleo dominante", "tu eje central también organiza temas de", profileThemes),
            BuildInsightMatch(analysis.EmotionalTone, "tono emocional", "tu mundo emocional también se mueve alrededor de", profileThemes),
            BuildInsightMatch(analysis.RelationalStyle, "forma de vincularte", "tu forma de vincularte también deja ver", profileThemes),
            BuildInsightMatch(analysis.ActionStyle, "forma de actuar", "tu forma de actuar también expresa", profileThemes)
        };

        return candidates
            .Where(x => x is not null)
            .OrderByDescending(x => x!.Score)
            .FirstOrDefault();
    }

    private static InsightMatch? FindBestContrastMatch(
        InterpretationAnalysisResult analysis,
        IReadOnlyDictionary<string, int> challengeThemes,
        IReadOnlyDictionary<string, int> desiredInsightThemes,
        InsightMatch? resonanceMatch)
    {
        var contrastSeed = MergeThemeScores(challengeThemes, desiredInsightThemes);

        if (contrastSeed.Count == 0)
        {
            return null;
        }

        var candidates = new[]
        {
            BuildInsightMatch(analysis.CentralTension, "tensión central", "la carta señala un ajuste alrededor de", contrastSeed),
            BuildInsightMatch(analysis.GrowthDirection, "dirección de crecimiento", "la carta abre una vía de trabajo ligada a", contrastSeed),
            BuildInsightMatch(analysis.EmotionalTone, "tono emocional", "la carta devuelve este tema al plano de", contrastSeed)
        };

        return candidates
            .Where(x => x is not null && x.Score > 0 && !string.Equals(x.InsightKey, resonanceMatch?.InsightKey, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x!.Score)
            .FirstOrDefault();
    }

    private static InsightMatch? BuildInsightMatch(
        AnalysisInsight insight,
        string insightLabel,
        string frame,
        IReadOnlyDictionary<string, int> profileThemes)
    {
        if (insight is null)
        {
            return null;
        }

        var insightThemes = ScoreThemes(
            insight.Headline,
            insight.Summary,
            string.Join(" ", insight.Keywords),
            string.Join(" ", insight.Signals));

        if (insightThemes.Count == 0)
        {
            return null;
        }

        var sharedThemes = EditorialThemes
            .Where(theme => profileThemes.ContainsKey(theme.Key) && insightThemes.ContainsKey(theme.Key))
            .OrderByDescending(theme => profileThemes[theme.Key] + insightThemes[theme.Key])
            .ToList();

        if (sharedThemes.Count == 0)
        {
            return null;
        }

        var score = sharedThemes.Sum(theme => Math.Min(profileThemes[theme.Key], insightThemes[theme.Key]));
        var highlightedThemes = JoinNatural(sharedThemes.Take(2).Select(x => x.Label).ToList());

        return new InsightMatch(
            insight.Key,
            insightLabel,
            $"{frame} {highlightedThemes}",
            score,
            sharedThemes);
    }

    private static IReadOnlyDictionary<string, int> ScoreThemes(params string?[] texts)
    {
        var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var corpus = NormalizeText(string.Join(" ", texts.Where(x => !string.IsNullOrWhiteSpace(x))));

        if (string.IsNullOrWhiteSpace(corpus))
        {
            return scores;
        }

        foreach (var theme in EditorialThemes)
        {
            var score = theme.Cues.Count(cue => corpus.Contains(cue, StringComparison.OrdinalIgnoreCase));
            if (score > 0)
            {
                scores[theme.Key] = score;
            }
        }

        return scores;
    }

    private static IReadOnlyDictionary<string, int> MergeThemeScores(
        IReadOnlyDictionary<string, int> first,
        IReadOnlyDictionary<string, int> second)
    {
        var merged = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in first)
        {
            merged[pair.Key] = pair.Value;
        }

        foreach (var pair in second)
        {
            merged[pair.Key] = merged.TryGetValue(pair.Key, out var current)
                ? current + pair.Value
                : pair.Value;
        }

        return merged;
    }

    private static bool ContainsAny(string value, params string[] tokens)
    {
        return tokens.Any(value.Contains);
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

    private static string TrimAndEnsureTerminal(string value, int maxLength)
    {
        var normalized = NormalizeText(value);
        if (normalized.Length > maxLength)
        {
            normalized = $"{normalized[..maxLength].TrimEnd()}...";
        }

        return EnsureTerminalPunctuation(normalized);
    }

    private static string EnsureTerminalPunctuation(string value)
    {
        var normalized = NormalizeText(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        return normalized[^1] is '.' or '!' or '?'
            ? normalized
            : $"{normalized}.";
    }

    private static string NormalizeText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var collapsed = string.Join(" ", value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var normalized = collapsed.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), @"\s+", " ").ToLowerInvariant();
    }
}
