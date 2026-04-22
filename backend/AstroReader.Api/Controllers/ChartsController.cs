using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly ICalculateNatalChartUseCase _calculateNatalChartUseCase;

    public ChartsController(ICalculateNatalChartUseCase calculateNatalChartUseCase)
    {
        _calculateNatalChartUseCase = calculateNatalChartUseCase;
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(CalculateChartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateChart([FromBody] CalculateChartRequest request, CancellationToken cancellationToken)
    {
        // El framework ASP.NET Core automáticamente valida los [Required] y atributos en el request
        // devolviendo un 400 Bad Request si no se cumplen.
        
        var response = await _calculateNatalChartUseCase.ExecuteAsync(request, cancellationToken);

        return Ok(response);
    }
}
