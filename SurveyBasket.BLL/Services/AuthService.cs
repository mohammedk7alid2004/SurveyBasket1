using Hangfire;

namespace SurveyBasket.BLL.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private const int RefreshTokenExpiryDays = 7;

    public async Task<Result<AuthResponse>> GetTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (!result.Succeeded)
            return Result.Failure<AuthResponse>(
                result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expireson = refreshTokenExpiry
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiry);

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(
        string token,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var existingRefreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (existingRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        existingRefreshToken.RevokedOn = DateTime.UtcNow;

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            Expireson = refreshTokenExpiry
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            newToken,
            expiresIn,
            newRefreshToken,
            refreshTokenExpiry);

        return Result.Success(response);
    }

    public async Task<Result> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailExists = await _userManager.Users
            .AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailExists)
            return Result.Failure(UserErrors.EmailAlreadyExists);

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(
                new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await SendConfirmationEmailAsync(user);
        return Result.Success();
    }

    public async Task<bool?> RevokedRefreshTokenAsync(
        string token,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var existingRefreshToken = user.RefreshTokens
            .SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);

        if (existingRefreshToken is null)
            return false;

        existingRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<Result> ConfirmEmailAsync(
        EmailConfrimRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(UserErrors.InvalidCode);
    }
    public async Task<Result> SendResetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken = default)
    {
       if(await _userManager.FindByEmailAsync(request.Email) is not { }  user)
                return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {Code}", code);
        await SendResetPasswordEmail(user, code);
        return Result.Success();

    }
    public async  Task<Result> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.email) is not { } user)
            return Result.Success();
        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);
        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));
            result = await _userManager.ResetPasswordAsync(user, code,request.NewPassword);
        }
        catch(FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }
        if(result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }
    public async Task<Result> ResendConfirmEmailAsync(
        ResendConfrimatioEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        await SendConfirmationEmailAsync(user);
        return Result.Success();
    }

    private async Task SendConfirmationEmailAsync(ApplicationUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {Code}", code);

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var confirmationLink = $"{origin}/auth/ConfrimationEmail?userId={user.Id}&code={code}";

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfrimation",
            new Dictionary<string, string>
            {
                { "{FirstName}", user.FirstName },
                { "{ConfirmationLink}", confirmationLink }
            });

        BackgroundJob.Enqueue(()=>_emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody));
        await Task.CompletedTask;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FirstName },
                { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password", emailBody));

        await Task.CompletedTask;
    }

   
}