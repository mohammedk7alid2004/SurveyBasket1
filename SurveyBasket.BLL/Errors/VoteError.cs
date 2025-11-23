namespace SurveyBasket.BLL.Errors;

public static class VoteError
{


    public static readonly Error VoteNotFound =
        new("Vote.NotFound", "No vote was found with the given ID.", StatusCodes.Status404NotFound);

    public static readonly Error VoteAlreadyExists =
        new("Vote.AlreadyExists", "A vote for this user and poll already exists.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidVoteData =
        new("Vote.InvalidData", "The vote data provided is invalid.", StatusCodes.Status400BadRequest);

    public static readonly Error PollClosed =
        new("Vote.PollClosed", "The poll is closed and no more votes can be submitted.", StatusCodes.Status400BadRequest);

    public static readonly Error NotAuthorized =
        new("Vote.NotAuthorized", "You are not authorized to submit or modify this vote.", StatusCodes.Status403Forbidden);

    public static readonly Error OptionNotFound =
        new("Vote.OptionNotFound", "The selected option does not belong to this poll.", StatusCodes.Status404NotFound);

    public static readonly Error VoteLimitExceeded =
        new("Vote.LimitExceeded", "You have exceeded the allowed number of votes for this poll.", StatusCodes.Status429TooManyRequests);
}

