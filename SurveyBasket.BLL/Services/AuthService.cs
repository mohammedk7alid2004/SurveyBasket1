using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SurveyBasket.BLL.Helpers;
using SurveyBasket.Contract.Contracts.Email;

namespace SurveyBasket.BLL.Services;
public class AuthService(
    UserManager<ApplicationUser>userManager,
    IJwtProvider jwtProvider,
    SignInManager<ApplicationUser>signInManager,
    ILogger<AuthService>logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly int RefreshTokenExpiryDays = 7;
    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        var result = await signInManager.PasswordSignInAsync(user , password , false , false );
        if (result.Succeeded)
        {
            var (token, ExpiresIn) = _jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expireson = refreshTokenExpiry
            });
            await _userManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, ExpiresIn, refreshToken, refreshTokenExpiry);
            return Result.Success(response);

        }
        else

            return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);


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
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {

        var EmailIsExist = await _userManager.Users.AnyAsync(x=>x.Email==request.Email,cancellationToken);
        if(EmailIsExist )
            return Result.Failure<AuthResponse>(UserErrors.EmailAlreadyExists);
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
       var result= await _userManager.CreateAsync(user,request.Password);
        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("confirmation code :{code}", code);
            // TO DO Email
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody= EmailBodyBuilder.GenerateEmailBody("EmailConfrimation",
                new Dictionary<string,string>
            {
                {"{FirstName}",user.FirstName},
                {"{ConfirmationLink}",$"{origin}/auth/ConfrimationEmail?userId={user.Id}&code={code}"}
            });
            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
            return Result.Success();

        }
        var errors= result.Errors.First();
        return Result.Failure<AuthResponse>(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
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
    public async Task<Result> ConfirmEmailAsync(EmailConfrimRequest request, CancellationToken cancellationToken = default)
    {
        if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode );
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);
        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        }
        catch(FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return Result.Success();
        return Result.Failure(UserErrors.InvalidCode);
    }
    public async Task<Result> ResendConfirmEmailAsync(ResendConfrimatioEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("confirmation code :{code}", code);
        // TO DO Email
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfrimation",
            new Dictionary<string, string>
        {
                {"{FirstName}",user.FirstName},
                {"{ConfirmationLink}",$"{origin}/auth/ConfrimationEmail?userId={user.Id}&code={code}"}
        });
        await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
        return Result.Success();

    }
    private static string GenerateRefreshToken()
    {
      return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

   
}
