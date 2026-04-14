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
        var hook =
            $"Tu carta ya muestra una combinación clara entre Sol en {sunSign}, Luna en {moonSign} y Ascendente en {ascendantSign}.";

        var energyCoreText = coverage.CoverageStatus switch
        {
            "partial" =>
                $"{hook} La lectura premium para esta carta tiene cobertura parcial: ya hay base editorial para algunas posiciones, pero todavía faltan entradas para componer todos los bloques con consistencia.",
            "fallback" when coverage.CoveredEntries.Count == 0 =>
                $"{hook} La lectura premium completa todavía no tiene cobertura editorial suficiente para esta combinación, aunque la base astral ya fue calculada correctamente.",
            _ =>
                $"{hook} La lectura premium no pudo componerse completamente en este momento, aunque la base astral ya fue calculada correctamente."
        };

        var coreText = coverage.CoverageStatus switch
        {
            "partial" =>
                "Ya podemos identificar partes del núcleo de la carta, pero todavía faltan piezas editoriales para integrar la tríada central con el resto de la lectura.",
            _ =>
                "Ya podemos identificar la tríada central de la carta, aunque esta versión todavía no logró componer una lectura premium completa."
        };

        var personalDynamicsText = coverage.CoverageStatus switch
        {
            "partial" =>
                "La lectura de Mercurio, Venus y Marte todavía no tiene cobertura suficiente para transformarse en una síntesis premium consistente.",
            _ =>
                "La estructura premium para Mercurio, Venus y Marte no pudo componerse en esta respuesta."
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
                MainText = "La carta está calculada con motor real, pero esta respuesta no debe leerse como una lectura premium completa mientras la cobertura siga siendo parcial o de fallback."
            },
            TensionsAndPotential = [],
            LifeAreas = [],
            Profiles = [],
            Closing = "La base astral está disponible; lo que falta aquí es cobertura editorial completa o una composición premium exitosa."
        };
    }
}
