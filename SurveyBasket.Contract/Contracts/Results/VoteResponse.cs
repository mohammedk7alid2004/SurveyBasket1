namespace SurveyBasket.Contract.Contracts.Results;
public record VoteResponse
(
    string VoteName,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);
