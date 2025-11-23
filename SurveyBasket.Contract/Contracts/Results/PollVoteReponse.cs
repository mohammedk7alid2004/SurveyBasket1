namespace SurveyBasket.Contract.Contracts.Results;

public record PollVoteReponse
(
    string Title,
    IEnumerable<VoteResponse>Votes
);
