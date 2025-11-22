using Microsoft.AspNetCore.Http;

namespace SurveyBasket.BLL.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password.", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token.", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token.", StatusCodes.Status401Unauthorized);

    public static readonly Error UserNotFound =
        new("User.NotFound", "User not found.", StatusCodes.Status404NotFound);

    public static readonly Error EmailAlreadyExists =
        new("User.EmailAlreadyExists", "This email is already registered.", StatusCodes.Status409Conflict);

    public static readonly Error TokenExpired =
        new("User.TokenExpired", "Your token has expired. Please log in again.", StatusCodes.Status401Unauthorized);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Please confirm your email before logging in.", StatusCodes.Status403Forbidden);
}
