namespace SurveyBasket.Api.Controllers;

using SurveyBasket.Contract.Contracts.Email;
using ResetPasswordRequest = Contract.Contracts.Authentication.ResetPasswordRequest;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request , [FromServices] IAuthService authService, CancellationToken cancellationToken = default)
    {
        var authResult= await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
    }
    [HttpPost("register")]
    public async Task<IActionResult>Register(Contract.Contracts.Authentication.RegisterRequest request ,CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfrimRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.ConfirmEmailAsync(request,cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest();

    }
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfrimatioEmailRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.ResendConfirmEmailAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("forget-password")]
    public async Task<IActionResult>ForgetPassword(ForgetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.SendResetPasswordAsync(request, cancellationToken);
        return result.IsSuccess? Ok() : result.ToProblem();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPassword(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RevokedRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        
            return result is null ? Unauthorized():NoContent();
    }
}
