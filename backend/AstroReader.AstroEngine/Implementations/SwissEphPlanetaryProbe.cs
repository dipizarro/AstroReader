using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine.Implementations;

public sealed class SwissEphPlanetaryProbe : ISwissEphPlanetaryProbe
{
    private readonly SwissEphOptions _options;

    public SwissEphPlanetaryProbe(IOptions<SwissEphOptions> options)
    {
        _options = options.Value;
    }

    public PlanetLongitudeResult CalculateEclipticLongitudeUtc(DateTime utcDateTime, int planetId)
    {
        using var swiss = new SwissEphReflectionBridge();
        swiss.ConfigureEphemerisPath(_options.EphemerisPath);

        var julianDayUt = swiss.CalculateJulianDayUt(utcDateTime);
        var flags = swiss.GetConstant("SEFLG_SWIEPH") | swiss.GetConstant("SEFLG_SPEED");
        var calculation = swiss.CalculatePlanetLongitude(julianDayUt, planetId, flags);

        if (calculation.ReturnFlag < 0)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(calculation.ErrorText)
                    ? "Swiss Ephemeris devolvió un error al calcular la longitud eclíptica."
                    : $"Swiss Ephemeris devolvió un error: {calculation.ErrorText}");
        }

        return new PlanetLongitudeResult
        {
            UtcDateTime = NormalizeUtc(utcDateTime),
            PlanetId = planetId,
            JulianDayUt = julianDayUt,
            EclipticLongitude = NormalizeDegrees(calculation.Positions[0]),
            LongitudeSpeed = calculation.Positions[3],
            IsRetrograde = calculation.Positions[3] < 0,
            FlagsUsed = flags,
            EphemerisPath = _options.EphemerisPath,
            Warning = string.IsNullOrWhiteSpace(calculation.ErrorText) ? null : calculation.ErrorText
        };
    }

    private static DateTime NormalizeUtc(DateTime utcDateTime)
    {
        return utcDateTime.Kind switch
        {
            DateTimeKind.Utc => utcDateTime,
            DateTimeKind.Local => utcDateTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)
        };
    }

    private static double NormalizeDegrees(double degrees)
    {
        return (degrees % 360 + 360) % 360;
    }
}
