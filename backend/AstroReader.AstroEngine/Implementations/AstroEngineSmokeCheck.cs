using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Constants;
using AstroReader.AstroEngine.Contracts;
using AstroReader.AstroEngine.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine.Implementations;

internal sealed class AstroEngineSmokeCheck : IAstroEngineSmokeCheck
{
    private static readonly AstroCalculationRequest ProbeRequest = new(
        UtcDateTime: new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        Latitude: 51.4769,
        Longitude: 0d);

    private readonly IAstroCalculationEngine _astroCalculationEngine;
    private readonly SwissEphOptions _options;
    private readonly ILogger<AstroEngineSmokeCheck> _logger;

    public AstroEngineSmokeCheck(
        IAstroCalculationEngine astroCalculationEngine,
        IOptions<SwissEphOptions> options,
        ILogger<AstroEngineSmokeCheck> logger)
    {
        _astroCalculationEngine = astroCalculationEngine;
        _options = options.Value;
        _logger = logger;
    }

    public AstroEngineSmokeCheckResult Run()
    {
        var activeEngine = _options.GetConfiguredEngineName();
        var ephemerisPath = _options.GetEphemerisPathForLogs();

        if (!_options.ShouldUseSwissEph())
        {
            return new AstroEngineSmokeCheckResult
            {
                IsHealthy = true,
                ActiveEngine = activeEngine,
                HouseSystem = _options.HouseSystem,
                EphemerisPath = ephemerisPath,
                Message = "Smoke check omitido porque el engine activo es Mock.",
                Skipped = true
            };
        }

        try
        {
            var result = _astroCalculationEngine.Calculate(ProbeRequest);
            ValidateResult(result);

            _logger.LogInformation(
                "Astro engine smoke check passed. ActiveEngine={ActiveEngine}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}",
                activeEngine,
                _options.HouseSystem,
                ephemerisPath);

            return new AstroEngineSmokeCheckResult
            {
                IsHealthy = true,
                ActiveEngine = activeEngine,
                HouseSystem = _options.HouseSystem,
                EphemerisPath = ephemerisPath,
                Message = "Swiss Ephemeris respondió correctamente a un cálculo básico de prueba."
            };
        }
        catch (AstroCalculationException exception)
        {
            _logger.LogError(
                exception,
                "Astro engine smoke check failed. ActiveEngine={ActiveEngine}, Code={Code}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}",
                activeEngine,
                exception.Code,
                _options.HouseSystem,
                ephemerisPath);

            return new AstroEngineSmokeCheckResult
            {
                IsHealthy = false,
                ActiveEngine = activeEngine,
                HouseSystem = _options.HouseSystem,
                EphemerisPath = ephemerisPath,
                Message = exception.PublicMessage,
                ErrorCode = exception.Code
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Astro engine smoke check failed with an unexpected exception. ActiveEngine={ActiveEngine}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}",
                activeEngine,
                _options.HouseSystem,
                ephemerisPath);

            return new AstroEngineSmokeCheckResult
            {
                IsHealthy = false,
                ActiveEngine = activeEngine,
                HouseSystem = _options.HouseSystem,
                EphemerisPath = ephemerisPath,
                Message = "El motor astral no pudo completar un cálculo técnico básico.",
                ErrorCode = AstroCalculationErrorCode.Calculation
            };
        }
    }

    private static void ValidateResult(AstroCalculationResult result)
    {
        if (result.PlanetaryPositions.Count < SwissEphPlanetIds.CorePlanets.Length)
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Calculation,
                publicMessage: "El motor astral no pudo completar un cálculo técnico básico.",
                diagnosticMessage:
                $"El smoke check devolvió solo {result.PlanetaryPositions.Count} posiciones planetarias; se esperaban al menos {SwissEphPlanetIds.CorePlanets.Length}.");
        }

        if (result.Houses.Count < 12)
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Calculation,
                publicMessage: "El motor astral no pudo completar un cálculo técnico básico.",
                diagnosticMessage: $"El smoke check devolvió solo {result.Houses.Count} casas; se esperaban 12.");
        }

        if (result.AscendantDegree < 0d || result.AscendantDegree >= 360d)
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Calculation,
                publicMessage: "El motor astral no pudo completar un cálculo técnico básico.",
                diagnosticMessage: $"El ascendente del smoke check quedó fuera de rango: {result.AscendantDegree}.");
        }
    }
}
