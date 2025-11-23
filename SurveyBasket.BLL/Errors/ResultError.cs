
namespace SurveyBasket.BLL.Errors;

public static class ResultError
{
    public static readonly Error Failure =
        new("Result.Failure", "The operation failed to complete.", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidData =
        new("Result.InvalidData", "The provided data is invalid or incomplete.", StatusCodes.Status400BadRequest);

    public static readonly Error NotFound =
        new("Result.NotFound", "The requested resource was not found.", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Result.Unauthorized", "You are not authorized to perform this action.", StatusCodes.Status401Unauthorized);

    public static readonly Error Forbidden =
        new("Result.Forbidden", "You do not have permission to access this resource.", StatusCodes.Status403Forbidden);

    public static readonly Error Conflict =
        new("Result.Conflict", "A conflict occurred while processing the request.", StatusCodes.Status409Conflict);

    public static readonly Error ServerError =
        new("Result.ServerError", "An unexpected server error occurred.", StatusCodes.Status500InternalServerError);
}
