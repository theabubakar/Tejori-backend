using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;

namespace Tijori.API.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUserService;

    public ProfileController(IProfileService profileService, ICurrentUserService currentUserService)
    {
        _profileService = profileService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _profileService.GetProfileAsync(userId, cancellationToken);
        return Ok(ApiResponse<ProfileDto>.Ok(result, "Profile fetched successfully."));
    }
}
