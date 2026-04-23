using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalProfilesController : ControllerBase
{
    private readonly ICreatePersonalProfileUseCase _createPersonalProfileUseCase;
    private readonly IGetPersonalProfilesUseCase _getPersonalProfilesUseCase;
    private readonly IGetPersonalProfileByIdUseCase _getPersonalProfileByIdUseCase;
    private readonly IGetPersonalProfileBySavedChartIdUseCase _getPersonalProfileBySavedChartIdUseCase;
    private readonly IUpdatePersonalProfileUseCase _updatePersonalProfileUseCase;

    public PersonalProfilesController(
        ICreatePersonalProfileUseCase createPersonalProfileUseCase,
        IGetPersonalProfilesUseCase getPersonalProfilesUseCase,
        IGetPersonalProfileByIdUseCase getPersonalProfileByIdUseCase,
        IGetPersonalProfileBySavedChartIdUseCase getPersonalProfileBySavedChartIdUseCase,
        IUpdatePersonalProfileUseCase updatePersonalProfileUseCase)
    {
        _createPersonalProfileUseCase = createPersonalProfileUseCase;
        _getPersonalProfilesUseCase = getPersonalProfilesUseCase;
        _getPersonalProfileByIdUseCase = getPersonalProfileByIdUseCase;
        _getPersonalProfileBySavedChartIdUseCase = getPersonalProfileBySavedChartIdUseCase;
        _updatePersonalProfileUseCase = updatePersonalProfileUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonalProfileListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfiles([FromQuery] Guid? ownerUserId, CancellationToken cancellationToken)
    {
        var profiles = await _getPersonalProfilesUseCase.ExecuteAsync(ownerUserId, cancellationToken);
        return Ok(profiles);
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PersonalProfileDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateProfile(
        Guid id,
        [FromBody] UpdatePersonalProfileRequest request,
        [FromQuery] Guid? ownerUserId,
        CancellationToken cancellationToken)
    {
        var profile = await _updatePersonalProfileUseCase.ExecuteAsync(id, request, ownerUserId, cancellationToken);
        return Ok(profile);
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
