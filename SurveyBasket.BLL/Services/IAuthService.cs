namespace SurveyBasket.BLL.Services;
public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool?> RevokedRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}
