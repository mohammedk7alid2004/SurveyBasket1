using SurveyBasket.Contract.Contracts.Email;

namespace SurveyBasket.BLL.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool?> RevokedRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(EmailConfrimRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmEmailAsync(ResendConfrimatioEmailRequest request, CancellationToken cancellationToken = default);

}



