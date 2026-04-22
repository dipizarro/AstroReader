using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalProfilesController : ControllerBase
{
    private readonly ICreatePersonalProfileUseCase _createPersonalProfileUseCase;
    private readonly IGetPersonalProfileByIdUseCase _getPersonalProfileByIdUseCase;
    private readonly IGetPersonalProfileBySavedChartIdUseCase _getPersonalProfileBySavedChartIdUseCase;

    public PersonalProfilesController(
        ICreatePersonalProfileUseCase createPersonalProfileUseCase,
        IGetPersonalProfileByIdUseCase getPersonalProfileByIdUseCase,
        IGetPersonalProfileBySavedChartIdUseCase getPersonalProfileBySavedChartIdUseCase)
    {
        _createPersonalProfileUseCase = createPersonalProfileUseCase;
        _getPersonalProfileByIdUseCase = getPersonalProfileByIdUseCase;
        _getPersonalProfileBySavedChartIdUseCase = getPersonalProfileBySavedChartIdUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonalProfileDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateProfile(
        [FromBody] CreatePersonalProfileRequest request,
        CancellationToken cancellationToken)
    {
        var profile = await _createPersonalProfileUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetProfileById), new { id = profile.Id }, profile);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PersonalProfileDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileById(Guid id, [FromQuery] Guid? ownerUserId, CancellationToken cancellationToken)
    {
        var profile = await _getPersonalProfileByIdUseCase.ExecuteAsync(id, ownerUserId, cancellationToken);
        return Ok(profile);
    }

    [HttpGet("by-saved-chart/{savedChartId:guid}")]
    [ProducesResponseType(typeof(PersonalProfileDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileBySavedChartId(
        Guid savedChartId,
        [FromQuery] Guid? ownerUserId,
        CancellationToken cancellationToken)
    {
        var profile = await _getPersonalProfileBySavedChartIdUseCase.ExecuteAsync(
            savedChartId,
            ownerUserId,
            cancellationToken);

        return Ok(profile);
    }
}
