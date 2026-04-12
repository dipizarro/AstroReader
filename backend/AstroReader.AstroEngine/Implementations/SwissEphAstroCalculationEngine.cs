using AstroReader.AstroEngine.Constants;
using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using AstroReader.AstroEngine.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine.Implementations;

/// <summary>
/// Spike experimental: usa Swiss Ephemeris para posiciones planetarias reales,
/// ascendente y casas útiles para la primera carta natal real.
/// El mock sigue siendo la implementación default hasta que validemos el motor.
/// </summary>
internal sealed class SwissEphAstroCalculationEngine : IAstroCalculationEngine
{
    private readonly IAstroLongitudeProbe _longitudeProbe;
    private readonly ISwissEphClientFactory _swissEphClientFactory;
    private readonly SwissEphOptions _options;
    private readonly ILogger<SwissEphAstroCalculationEngine> _logger;

    public SwissEphAstroCalculationEngine(
        IAstroLongitudeProbe longitudeProbe,
        ISwissEphClientFactory swissEphClientFactory,
        IOptions<SwissEphOptions> options,
        ILogger<SwissEphAstroCalculationEngine> logger)
    {
        _longitudeProbe = longitudeProbe;
        _swissEphClientFactory = swissEphClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public AstroCalculationResult Calculate(AstroCalculationRequest request)
    {
        var planetaryPositions = new Dictionary<int, AsteroidalData>();

        foreach (var planetId in SwissEphPlanetIds.CorePlanets)
        {
            var result = _longitudeProbe.CalculateEclipticLongitudeUtc(request.UtcDateTime, planetId);
            planetaryPositions[planetId] = new AsteroidalData(
                result.EclipticLongitude,
                result.ZodiacSignIndex,
                result.SignDegree,
                result.IsRetrograde);
        }

        using var swiss = _swissEphClientFactory.CreateClient();
        var julianDayUt = swiss.CalculateJulianDayUt(request.UtcDateTime);
        var housesCalculation = swiss.CalculateHouses(julianDayUt, request.Latitude, request.Longitude);

        _logger.LogDebug(
            "SwissEph calculation completed. PlanetCount={PlanetCount}, HouseCount={HouseCount}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}",
            planetaryPositions.Count,
            housesCalculation.HouseCusps.Count,
            _options.HouseSystem,
            _options.GetEphemerisPathForLogs());

        return new AstroCalculationResult
        {
            AscendantDegree = housesCalculation.AscendantDegree,
            PlanetaryPositions = planetaryPositions,
            Houses = housesCalculation.HouseCusps
        };
    }
}
