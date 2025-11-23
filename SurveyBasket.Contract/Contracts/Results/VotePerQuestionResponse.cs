namespace SurveyBasket.Contract.Contracts.Results;

public record VotePerQuestionResponse
(
    string Question,
    IEnumerable<VotePerAnswerReponse> SeletedAnswers
);