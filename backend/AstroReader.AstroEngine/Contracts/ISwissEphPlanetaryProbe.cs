namespace AstroReader.AstroEngine.Contracts;

/// <summary>
/// Servicio de spike para validar Swiss Ephemeris sin reemplazar todavía el flujo principal.
/// </summary>
public interface ISwissEphPlanetaryProbe
{
    PlanetLongitudeResult CalculateEclipticLongitudeUtc(DateTime utcDateTime, int planetId);
}
