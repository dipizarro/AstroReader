using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavedChartsController : ControllerBase
{
    private readonly ISaveChartUseCase _saveChartUseCase;
    private readonly IGetSavedChartByIdUseCase _getSavedChartByIdUseCase;
    private readonly IGetSavedChartsUseCase _getSavedChartsUseCase;

    public SavedChartsController(
        ISaveChartUseCase saveChartUseCase,
        IGetSavedChartByIdUseCase getSavedChartByIdUseCase,
        IGetSavedChartsUseCase getSavedChartsUseCase)
    {
        _saveChartUseCase = saveChartUseCase;
        _getSavedChartByIdUseCase = getSavedChartByIdUseCase;
        _getSavedChartsUseCase = getSavedChartsUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SavedChartDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveChart([FromBody] SaveChartRequest request, CancellationToken cancellationToken)
    {
        var savedChart = await _saveChartUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetSavedChartById), new { id = savedChart.Id }, savedChart);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SavedChartListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavedCharts([FromQuery] Guid? ownerUserId, CancellationToken cancellationToken)
    {
        var savedCharts = await _getSavedChartsUseCase.ExecuteAsync(ownerUserId, cancellationToken);
        return Ok(savedCharts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SavedChartDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSavedChartById(Guid id, [FromQuery] Guid? ownerUserId, CancellationToken cancellationToken)
    {
        var savedChart = await _getSavedChartByIdUseCase.ExecuteAsync(id, ownerUserId, cancellationToken);
        return Ok(savedChart);
    }
}
