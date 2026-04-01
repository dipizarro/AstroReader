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
    public IActionResult CalculateChart([FromBody] CalculateChartRequest request)
    {
        // El framework ASP.NET Core automáticamente valida los [Required] y atributos en el request
        // devolviendo un 400 Bad Request si no se cumplen.
        
        var response = _calculateNatalChartUseCase.Execute(request);

        return Ok(response);
    }
}
