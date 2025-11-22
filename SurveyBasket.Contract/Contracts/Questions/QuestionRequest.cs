namespace SurveyBasket.Contract.Contracts.Questions;

public record QuestionRequest
(
    string Content,
    List<string>Answers
);
