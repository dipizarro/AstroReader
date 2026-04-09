using AstroReader.AstroEngine.Constants;
using AstroReader.AstroEngine.Contracts;

namespace AstroReader.AstroEngine.Implementations;

/// <summary>
/// Spike experimental: usa Swiss Ephemeris para posiciones planetarias reales,
/// pero todavía no resuelve ascendente ni casas.
/// El mock sigue siendo la implementación default hasta que validemos el motor.
/// </summary>
public sealed class SwissEphAstroCalculationEngine : IAstroCalculationEngine
{
    private readonly ISwissEphPlanetaryProbe _planetaryProbe;

    public SwissEphAstroCalculationEngine(ISwissEphPlanetaryProbe planetaryProbe)
    {
        _planetaryProbe = planetaryProbe;
    }

    public AstroCalculationResult Calculate(AstroCalculationRequest request)
    {
        var planetaryPositions = new Dictionary<int, AsteroidalData>();

        foreach (var planetId in SwissEphPlanetIds.CorePlanets)
        {
            var result = _planetaryProbe.CalculateEclipticLongitudeUtc(request.UtcDateTime, planetId);
            planetaryPositions[planetId] = new AsteroidalData(result.EclipticLongitude, result.IsRetrograde);
        }

        return new AstroCalculationResult
        {
            // Sprint spike: ascendente y casas quedarán para la siguiente fase.
            AscendantDegree = 0,
            PlanetaryPositions = planetaryPositions,
            Houses = new Dictionary<int, double>()
        };
    }
}
