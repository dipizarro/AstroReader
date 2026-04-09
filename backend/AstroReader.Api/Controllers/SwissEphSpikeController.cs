using AstroReader.AstroEngine.Constants;
using AstroReader.AstroEngine.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/swiss-eph-spike")]
public class SwissEphSpikeController : ControllerBase
{
    private readonly IAstroLongitudeProbe _longitudeProbe;

    public SwissEphSpikeController(IAstroLongitudeProbe longitudeProbe)
    {
        _longitudeProbe = longitudeProbe;
    }

    [HttpGet("planet-longitude")]
    [ProducesResponseType(typeof(PlanetLongitudeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetPlanetLongitude(
        [FromQuery] DateTime utcDateTime,
        [FromQuery] string planet = "sun")
    {
        if (utcDateTime == default)
        {
            return BadRequest("Debes indicar utcDateTime en formato ISO-8601, por ejemplo 2024-01-01T12:00:00Z.");
        }

        var planetId = planet.Trim().ToLowerInvariant() switch
        {
            "sun" => SwissEphPlanetIds.Sun,
            "moon" => SwissEphPlanetIds.Moon,
            "mercury" => SwissEphPlanetIds.Mercury,
            "venus" => SwissEphPlanetIds.Venus,
            "mars" => SwissEphPlanetIds.Mars,
            "jupiter" => SwissEphPlanetIds.Jupiter,
            "saturn" => SwissEphPlanetIds.Saturn,
            _ => throw new ArgumentException("Planet inválido. Usa: sun, moon, mercury, venus, mars, jupiter o saturn.")
        };

        var result = _longitudeProbe.CalculateEclipticLongitudeUtc(
            utcDateTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)
                : utcDateTime.ToUniversalTime(),
            planetId);

        return Ok(result);
    }
}
