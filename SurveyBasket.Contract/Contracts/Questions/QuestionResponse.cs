using SurveyBasket.Contract.Contracts.Answers;

namespace SurveyBasket.Contract.Contracts.Questions;

public record QuestionResponse
(
    int Id ,
    string Content,
    IEnumerable<AnswerResponse>Answers
);
