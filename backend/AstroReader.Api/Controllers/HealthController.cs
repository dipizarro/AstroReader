using System.Runtime.InteropServices;
using AstroReader.AstroEngine.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IAstroEngineSmokeCheck _astroEngineSmokeCheck;
    private readonly IAstroEngineTechnicalMetadataProvider _astroEngineTechnicalMetadataProvider;

    public HealthController(
        IAstroEngineSmokeCheck astroEngineSmokeCheck,
        IAstroEngineTechnicalMetadataProvider astroEngineTechnicalMetadataProvider)
    {
        _astroEngineSmokeCheck = astroEngineSmokeCheck;
        _astroEngineTechnicalMetadataProvider = astroEngineTechnicalMetadataProvider;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var smokeCheck = _astroEngineSmokeCheck.Run();
        var metadata = _astroEngineTechnicalMetadataProvider.GetCurrent();
        var status = smokeCheck.IsHealthy ? "healthy" : "degraded";

        var payload = new
        {
            status,
            api = "running",
            astroEngine = new
            {
                activeEngine = metadata.CalculationEngine,
                operable = smokeCheck.IsHealthy
            }
        };

        return smokeCheck.IsHealthy
            ? Ok(payload)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }

    [HttpGet("astro-engine")]
    public IActionResult GetAstroEngineDiagnostics()
    {
        var smokeCheck = _astroEngineSmokeCheck.Run();
        var metadata = _astroEngineTechnicalMetadataProvider.GetCurrent();

        var payload = new
        {
            activeEngine = metadata.CalculationEngine,
            usesRealEngine = metadata.UsesRealEngine,
            houseSystem = metadata.HouseSystemCode,
            wrapperVersion = metadata.WrapperVersion,
            ephemeris = new
            {
                customPathConfigured = metadata.UsesCustomEphemerisPath,
                effectivePath = smokeCheck.EphemerisPath
            },
            runtime = new
            {
                os = RuntimeInformation.OSDescription,
                processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                framework = RuntimeInformation.FrameworkDescription
            },
            operability = new
            {
                ok = smokeCheck.IsHealthy,
                skipped = smokeCheck.Skipped,
                code = smokeCheck.ErrorCode?.ToString(),
                message = smokeCheck.Message
            }
        };

        return smokeCheck.IsHealthy
            ? Ok(payload)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }
}
