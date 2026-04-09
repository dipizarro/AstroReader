using System.ComponentModel.DataAnnotations;

namespace AstroReader.AstroEngine.Configuration;

public sealed record SwissEphOptions
{
    public const string SectionName = "AstroEngine:SwissEph";
    public const string DefaultHouseSystem = "P";
    public const string MockEngineMode = "Mock";
    public const string SwissEphEngineMode = "SwissEph";

    /// <summary>
    /// Selector explícito del engine astral durante el spike.
    /// Valores admitidos: Mock, SwissEph.
    /// </summary>
    [MaxLength(32)]
    public string CalculationEngine { get; init; } = MockEngineMode;

    /// <summary>
    /// Compatibilidad temporal con la activación inicial del spike.
    /// Si viene true y no se configuró CalculationEngine, se activa SwissEph.
    /// </summary>
    public bool EnableSwissEphForNatalCharts { get; init; }

    /// <summary>
    /// Directorio donde vivirán los archivos de efemérides (.se1, .se2, etc).
    /// En este spike lo dejamos opcional para poder usar el fallback interno del wrapper.
    /// </summary>
    [MaxLength(500)]
    public string? EphemerisPath { get; init; }

    /// <summary>
    /// Sistema de casas Swiss Ephemeris. Para MVP usamos Placidus ("P") por defecto.
    /// </summary>
    [MaxLength(1)]
    public string HouseSystem { get; init; } = DefaultHouseSystem;

    public bool ShouldUseSwissEph()
    {
        var normalizedMode = (CalculationEngine ?? string.Empty).Trim();

        if (normalizedMode.Equals(SwissEphEngineMode, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (normalizedMode.Equals(MockEngineMode, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(normalizedMode))
        {
            return EnableSwissEphForNatalCharts;
        }

        throw new InvalidOperationException(
            $"AstroEngine:SwissEph:CalculationEngine='{CalculationEngine}' no es válido. Usa '{MockEngineMode}' o '{SwissEphEngineMode}'.");
    }
}
