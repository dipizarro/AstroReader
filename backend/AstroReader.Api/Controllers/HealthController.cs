using AstroReader.AstroEngine.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IAstroEngineSmokeCheck _astroEngineSmokeCheck;

    public HealthController(IAstroEngineSmokeCheck astroEngineSmokeCheck)
    {
        _astroEngineSmokeCheck = astroEngineSmokeCheck;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var smokeCheck = _astroEngineSmokeCheck.Run();
        var status = smokeCheck.IsHealthy ? "healthy" : "degraded";

        var payload = new
        {
            status,
            api = "running",
            astroEngine = new
            {
                activeEngine = smokeCheck.ActiveEngine,
                houseSystem = smokeCheck.HouseSystem,
                ephemerisPath = smokeCheck.EphemerisPath,
                smokeCheck = new
                {
                    ok = smokeCheck.IsHealthy,
                    skipped = smokeCheck.Skipped,
                    code = smokeCheck.ErrorCode?.ToString(),
                    message = smokeCheck.Message
                }
            }
        };

        return smokeCheck.IsHealthy
            ? Ok(payload)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }
}
