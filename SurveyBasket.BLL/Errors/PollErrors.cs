using Microsoft.AspNetCore.Http;

namespace SurveyBasket.BLL.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "No poll was found with the given ID.", StatusCodes.Status404NotFound);

    public static readonly Error PollAlreadyExists =
        new("Poll.AlreadyExists", "A poll with the same title already exists.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidPollData =
        new("Poll.InvalidData", "The poll data provided is invalid.", StatusCodes.Status400BadRequest);

    public static readonly Error PollClosed =
        new("Poll.Closed", "This poll is already closed and cannot be modified.", StatusCodes.Status400BadRequest);

    public static readonly Error OptionNotFound =
        new("Poll.OptionNotFound", "The selected option was not found in this poll.", StatusCodes.Status404NotFound);

    public static readonly Error NotAuthorized =
        new("Poll.NotAuthorized", "You are not authorized to modify or delete this poll.", StatusCodes.Status403Forbidden);

    public static readonly Error VoteAlreadySubmitted =
        new("Poll.VoteAlreadySubmitted", "You have already voted in this poll.", StatusCodes.Status409Conflict);

    public static readonly Error VoteLimitExceeded =
        new("Poll.VoteLimitExceeded", "You have reached the maximum number of allowed votes.", StatusCodes.Status429TooManyRequests);
}
