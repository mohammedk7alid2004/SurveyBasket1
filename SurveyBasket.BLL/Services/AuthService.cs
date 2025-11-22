namespace SurveyBasket.BLL.Services;
public class AuthService(UserManager<ApplicationUser>userManager,IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int RefreshTokenExpiryDays = 7;
    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
      var IsValidPassword= await _userManager.CheckPasswordAsync(user, password);
        if(!IsValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        var (token, ExpiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expireson = refreshTokenExpiry
        });
        await _userManager.UpdateAsync(user);
        var result =new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, ExpiresIn, refreshToken, refreshTokenExpiry);
        return Result.Success(result);


    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId == null)
            Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            Result.Failure<AuthResponse>(UserErrors.UserNotFound);
        var existingRefreshToken = user!.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (existingRefreshToken == null)
            Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        var (newToken, ExpiresIn) = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        existingRefreshToken!.RevokedOn = DateTime.UtcNow;
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            Expireson = refreshTokenExpiry
        });
        await _userManager.UpdateAsync(user);
        var result = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, ExpiresIn, newRefreshToken, refreshTokenExpiry);
        return Result.Success(result);
    }
    public async  Task<bool?> RevokedRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId == null)
            return false;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;
        var existingRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (existingRefreshToken == null)
            return false;
        existingRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return true;
    }
    private static string GenerateRefreshToken()
    {
      return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    
}
