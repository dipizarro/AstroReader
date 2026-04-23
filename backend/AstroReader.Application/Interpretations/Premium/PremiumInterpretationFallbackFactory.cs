using AstroReader.Application.Charts.DTOs;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationFallbackFactory
{
    public static ChartInterpretation Create(
        ZodiacSign sunSign,
        ZodiacSign moonSign,
        ZodiacSign ascendantSign,
        InterpretationCoverage coverage,
        PremiumReaderProfileContext? readerProfile = null)
    {
        var baseTriad =
            $"Sol en {sunSign}, Luna en {moonSign} y Ascendente en {ascendantSign}";
        var hasAnyEditorialCoverage = coverage.CoveredEntries.Count > 0;

        var hook =
            $"Esta respuesta te entrega una base de lectura construida sobre {baseTriad}, aunque todavía no corresponde a una interpretación premium completa.";

        var energyCoreText = hasAnyEditorialCoverage
            ? "La carta conserva algunas piezas editoriales disponibles, pero no alcanzan una base suficiente para presentarla como lectura premium parcial. Por eso esta respuesta se muestra como una lectura base."
            : "Por ahora lo más honesto es mostrar esta carta como una base de lectura. El cálculo astral está resuelto correctamente, pero todavía no hay cobertura editorial suficiente para presentarla como una interpretación premium real.";

        const string coreText =
            "Ya es posible reconocer el tono general de la carta desde su núcleo, pero todavía no hay suficiente desarrollo editorial para convertir eso en una lectura premium consistente.";

        const string personalDynamicsText =
            "La parte de pensamiento, vínculo y acción todavía no tiene base editorial suficiente como para mostrarse con profundidad premium en esta respuesta.";

        const string essentialSummaryText =
            "Esta salida funciona como una lectura base de producto. Sirve para orientar, pero no reemplaza la experiencia premium completa cuando el catálogo todavía no cubre esta combinación.";

        const string closing =
            "La base astral de la carta está disponible y es confiable. La diferencia pendiente está en el nivel editorial de la interpretación, no en el cálculo.";
        var contextProfiles = PremiumInterpretationProfileNarrative
            .BuildFallbackProfiles(readerProfile, sunSign, moonSign, ascendantSign)
            .Select(profile => new InterpretationProfileDto
            {
                Key = profile.Key,
                Title = profile.Title,
                Summary = profile.Summary
            })
            .ToList();

        return new ChartInterpretation
        {
            Coverage = coverage,
            Hook = PremiumInterpretationProfileNarrative.ApplyHookPersonalization(hook, readerProfile),
            EnergyCore = new InterpretationContentBlock
            {
                Key = "energyCore",
                Title = "Tu energía central",
                MainText = energyCoreText
            },
            Core = new InterpretationContentBlock
            {
                Key = "core",
                Title = "Tu núcleo",
                MainText = coreText
            },
            PersonalDynamics = new InterpretationContentBlock
            {
                Key = "personalDynamics",
                Title = "Tu forma de pensar, vincularte y actuar",
                MainText = personalDynamicsText
            },
            EssentialSummary = new InterpretationContentBlock
            {
                Key = "essentialSummary",
                Title = "Lo esencial de tu carta",
                MainText = essentialSummaryText
            },
            TensionsAndPotential = [],
            LifeAreas = [],
            Profiles = contextProfiles,
            Closing = PremiumInterpretationProfileNarrative.ApplyClosingPersonalization(closing, readerProfile)
        };
    }
}
