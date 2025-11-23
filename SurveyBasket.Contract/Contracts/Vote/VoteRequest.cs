namespace SurveyBasket.Contract.Contracts.Vote;

public record VoteRequest
(
    IEnumerable<VoteAnswerRequest> Answers
);
