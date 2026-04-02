namespace AstroReader.AstroEngine.Contracts;

/// <summary>
/// Contrato principal para cualquier motor astrológico enchufable (Swiss Ephemeris, JPL, etc).
/// Se ubica en AstroReader.AstroEngine.Contracts. Así, la capa de Application dependerá
/// únicamente de esta abstracción para realizar el trabajo pesado.
/// </summary>
public interface IAstroCalculationEngine
{
    /// <summary>
    /// Calcula posiciones matemáticas (planetas, aspectos, etc.) para un punto y fecha dados.
    /// </summary>
    AstroCalculationResult Calculate(AstroCalculationRequest request);
}
