using System.ComponentModel.DataAnnotations;

namespace AstroReader.AstroEngine.Configuration;

public sealed record SwissEphOptions
{
    public const string SectionName = "AstroEngine:SwissEph";
    public const string DefaultHouseSystem = "P";

    /// <summary>
    /// Mantiene el mock como default y permite activar SwissEph por configuración cuando el spike madure.
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
}
