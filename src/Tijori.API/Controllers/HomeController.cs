using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;

namespace Tijori.API.Controllers;

[Authorize]
[ApiController]
[Route("api/home")]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;
    private readonly ICurrentUserService _currentUserService;

    public HomeController(IHomeService homeService, ICurrentUserService currentUserService)
    {
        _homeService = homeService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HomeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHome(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _homeService.GetHomeAsync(userId, cancellationToken);
        return Ok(ApiResponse<HomeDto>.Ok(result, "Home data fetched successfully."));
    }
}
