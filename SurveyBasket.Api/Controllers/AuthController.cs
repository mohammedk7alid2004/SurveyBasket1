namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> GetTokenAsync([FromBody] LoginRequest request , [FromServices] IAuthService authService, CancellationToken cancellationToken = default)
    {
        var authResult= await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
        return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest();
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest();

    }
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RevokedRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        
            return result is null ? Unauthorized():NoContent();
    }
}
