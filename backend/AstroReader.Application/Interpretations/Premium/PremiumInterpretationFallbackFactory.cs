using AstroReader.Application.Charts.DTOs;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

internal static class PremiumInterpretationFallbackFactory
{
    public static ChartInterpretation Create(
        ZodiacSign sunSign,
        ZodiacSign moonSign,
        ZodiacSign ascendantSign,
        InterpretationCoverage coverage)
    {
        var baseTriad =
            $"Sol en {sunSign}, Luna en {moonSign} y Ascendente en {ascendantSign}";

        var hook = coverage.CoverageStatus switch
        {
            "partial" =>
                $"Esta carta ya tiene una lectura premium parcial construida sobre tu tríada central: {baseTriad}.",
            _ =>
                $"Esta respuesta te entrega una base de lectura construida sobre {baseTriad}, aunque todavía no corresponde a una interpretación premium completa."
        };

        var energyCoreText = coverage.CoverageStatus switch
        {
            "partial" =>
                $"La carta ya tiene cobertura suficiente para mostrar parte de su lectura con criterio editorial. Lo más firme por ahora está en la energía central y en la manera en que se organiza tu base personal, mientras el resto del catálogo sigue creciendo.",
            "fallback" when coverage.CoveredEntries.Count == 0 =>
                "Por ahora lo más honesto es mostrar esta carta como una base de lectura. El cálculo astral está resuelto correctamente, pero todavía no hay cobertura editorial suficiente para presentarla como una interpretación premium real.",
            _ =>
                "La carta conserva una base útil para orientar la lectura, pero esta respuesta no debe leerse como una interpretación premium terminada."
        };

        var coreText = coverage.CoverageStatus switch
        {
            "partial" =>
                "La tríada central ya permite una lectura valiosa del tono general de la carta, aunque todavía faltan capas para desarrollar una integración más completa.",
            _ =>
                "Ya es posible reconocer el tono general de la carta desde su núcleo, pero todavía no hay suficiente desarrollo editorial para convertir eso en una lectura premium consistente."
        };

        var personalDynamicsText = coverage.CoverageStatus switch
        {
            "partial" =>
                "La capa de pensamiento, vínculo y acción todavía está en desarrollo. Algunas piezas pueden empezar a insinuarse, pero no conviene presentarlas como una síntesis premium cerrada.",
            _ =>
                "La parte de pensamiento, vínculo y acción todavía no tiene base editorial suficiente como para mostrarse con profundidad premium en esta respuesta."
        };

        var essentialSummaryText = coverage.CoverageStatus switch
        {
            "partial" =>
                "Lo que ves aquí es una lectura útil y honesta de transición: ya hay composición real en algunos bloques, pero la experiencia premium completa todavía depende de mayor cobertura editorial.",
            _ =>
                "Esta salida funciona como una lectura base de producto. Sirve para orientar, pero no reemplaza la experiencia premium completa cuando el catálogo todavía no cubre esta combinación."
        };

        var closing = coverage.CoverageStatus switch
        {
            "partial" =>
                "La carta ya ofrece una base interpretativa real. Lo que falta ahora no es cálculo astral, sino más cobertura editorial para profundizar la lectura.",
            _ =>
                "La base astral de la carta está disponible y es confiable. La diferencia pendiente está en el nivel editorial de la interpretación, no en el cálculo."
        };

        return new ChartInterpretation
        {
            Coverage = coverage,
            Hook = hook,
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
            Profiles = [],
            Closing = closing
        };
    }
}
