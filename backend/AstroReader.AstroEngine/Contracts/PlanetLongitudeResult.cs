namespace AstroReader.AstroEngine.Contracts;

public sealed record PlanetLongitudeResult
{
    public DateTime UtcDateTime { get; init; }
    public int PlanetId { get; init; }
    public double JulianDayUt { get; init; }
    public double EclipticLongitude { get; init; }
    public double LongitudeSpeed { get; init; }
    public bool IsRetrograde { get; init; }
    public int FlagsUsed { get; init; }
    public string? EphemerisPath { get; init; }
    public string? Warning { get; init; }
}
