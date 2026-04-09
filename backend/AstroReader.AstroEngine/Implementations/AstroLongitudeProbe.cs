using AstroReader.AstroEngine.Contracts;
using AstroReader.AstroEngine.Internal;

namespace AstroReader.AstroEngine.Implementations;

internal sealed class AstroLongitudeProbe : IAstroLongitudeProbe
{
    private readonly ISwissEphClientFactory _clientFactory;

    public AstroLongitudeProbe(ISwissEphClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public PlanetLongitudeResult CalculateEclipticLongitudeUtc(DateTime utcDateTime, int planetId)
    {
        using var swiss = _clientFactory.CreateClient();

        var julianDayUt = swiss.CalculateJulianDayUt(utcDateTime);
        var flags = swiss.GetSwissEphemerisPlanetFlags();
        var calculation = swiss.CalculatePlanetLongitude(julianDayUt, planetId, flags);

        if (calculation.ReturnFlag < 0)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(calculation.ErrorText)
                    ? "No fue posible calcular la longitud eclíptica del planeta solicitado."
                    : "No fue posible calcular la longitud eclíptica del planeta solicitado.");
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
            EphemerisPath = swiss.EphemerisPath,
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
