namespace SurveyBasket.Contract.Contracts.Results;

public record VotePerDayResponse
(
    DateOnly Date,
    int NumberOfVotes
);
