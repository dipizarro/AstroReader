using System.Collections.Generic;

namespace AstroReader.AstroEngine.Contracts;

/// <summary>
/// Contiene el resultado puro del cálculo astronómico.
/// </summary>
public record AstroCalculationResult
{
    /// <summary>
    /// Grado absoluto en la eclíptica (0 a 360) del Ascendente.
    /// Clave para el cálculo de casas si se externaliza el frontend.
    /// </summary>
    public double AscendantDegree { get; init; }

    /// <summary>
    /// Posiciones absolutas de los planetas u otros cuerpos celestes.
    /// Key = ID interno del planeta (ej. constante de Swiss Ephemeris).
    /// </summary>
    public IReadOnlyDictionary<int, AsteroidalData> PlanetaryPositions { get; init; } = new Dictionary<int, AsteroidalData>();

    /// <summary>
    /// Cúspides de las casas astronómicas.
    /// Key = Número de la casa (1-12). Value = Grado absoluto (0 a 360).
    /// </summary>
    public IReadOnlyDictionary<int, double> Houses { get; init; } = new Dictionary<int, double>();
}

/// <summary>
/// Representa la data en bruto de un cuerpo celeste en el motor.
/// </summary>
public record AsteroidalData(
    double AbsoluteDegree,
    int ZodiacSignIndex,
    double SignDegree,
    bool IsRetrograde
);
