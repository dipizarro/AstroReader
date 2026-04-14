using AstroReader.Application.Interpretations.Premium;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/interpretations/premium")]
public class PremiumInterpretationsController : ControllerBase
{
    private readonly IPremiumInterpretationPreviewUseCase _premiumInterpretationPreviewUseCase;

    public PremiumInterpretationsController(
        IPremiumInterpretationPreviewUseCase premiumInterpretationPreviewUseCase)
    {
        _premiumInterpretationPreviewUseCase = premiumInterpretationPreviewUseCase;
    }

    [HttpPost("preview")]
    [ProducesResponseType(typeof(PremiumInterpretationPreviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Preview([FromBody] PremiumInterpretationPreviewRequest request)
    {
        var response = _premiumInterpretationPreviewUseCase.Execute(request);
        return Ok(response);
    }
}
