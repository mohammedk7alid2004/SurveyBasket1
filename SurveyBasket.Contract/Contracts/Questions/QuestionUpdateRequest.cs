namespace SurveyBasket.Contract.Contracts.Questions;

public record QuestionUpdateRequest
(
string Content,
List<string> Answers
);
