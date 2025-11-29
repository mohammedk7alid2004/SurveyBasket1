using SurveyBasket.Api.Extensions;
using SurveyBasket.Contract.Contracts.Users;

namespace SurveyBasket.Api.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult>Info()
    {
        var result = await _userService.GetProfileAsync(User.GetUserId()!);
        return Ok(result.Value);

    }
    [HttpPut("Info")]
    public async Task<IActionResult>Update(UpdateUserProfileRequest request)
    {
        var result= await _userService.UpdateProfileAsync(User.GetUserId()!, request);
        return NoContent();
    }
    [HttpPut("change-password")]
    public async  Task<IActionResult>ChangePassword (ChangePasswordRequest request)
    {
        var result= await _userService.ChangePasswordAsync(User.GetUserId()!, request);
        return result.IsSuccess? NoContent() :result.ToProblem();
    }
}
