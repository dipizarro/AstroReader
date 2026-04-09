namespace AstroReader.AstroEngine.Internal;

internal interface ISwissEphClient : IDisposable
{
    string? EphemerisPath { get; }

    double CalculateJulianDayUt(DateTime utcDateTime);
    int GetSwissEphemerisPlanetFlags();
    SwissPlanetCalculation CalculatePlanetLongitude(double julianDayUt, int planetId, int flags);
    SwissHouseCalculation CalculateHouses(double julianDayUt, double latitude, double longitude);
}
