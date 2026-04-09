namespace AstroReader.AstroEngine.Contracts;

/// <summary>
/// Contrato público de lectura astronómica puntual para spikes, validaciones y futuras herramientas internas.
/// El resto del sistema depende de este contrato, no de SwissEphNet.
/// </summary>
public interface IAstroLongitudeProbe
{
    PlanetLongitudeResult CalculateEclipticLongitudeUtc(DateTime utcDateTime, int planetId);
}
