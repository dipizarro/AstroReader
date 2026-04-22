namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationProfileNarrative
{
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

    public static IReadOnlyList<PremiumInterpretationProfile> BuildProfiles(PremiumInterpretationContext context)
    {
        if (context.ReaderProfile is null || !context.ReaderProfile.HasEditorialContext)
        {
            return [];
        }

        var profiles = new List<PremiumInterpretationProfile>
        {
            BuildContrastProfile(context)
        };

        var priorityProfile = BuildPriorityProfile(context);
        if (priorityProfile is not null)
        {
            profiles.Add(priorityProfile);
        }

        return profiles;
    }

    private static PremiumInterpretationProfile BuildContrastProfile(PremiumInterpretationContext context)
    {
        var profile = context.ReaderProfile!;
        var parts = new List<string>
        {
            $"Tu carta sigue marcando el tono de fondo desde Sol en {context.SunSign}, Luna en {context.MoonSign} y Ascendente en {context.AscendantSign}."
        };

        if (!string.IsNullOrWhiteSpace(profile.SelfPerceptionFocus))
        {
            parts.Add($"Hoy te reconoces especialmente en {EnsureTerminalPunctuation(LowercaseInitial(profile.SelfPerceptionFocus))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.CurrentChallenge))
        {
            parts.Add($"Ese foco convive con un desafío claro: {EnsureTerminalPunctuation(LowercaseInitial(profile.CurrentChallenge))}");
        }

        if (!string.IsNullOrWhiteSpace(profile.SelfDescription))
        {
            parts.Add($"Tu propia descripción añade una nota más íntima a esta lectura: {TrimAndEnsureTerminal(profile.SelfDescription!, 120)}");
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

    private static bool ContainsAny(string value, params string[] tokens)
    {
        return tokens.Any(value.Contains);
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
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Join(" ", value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
