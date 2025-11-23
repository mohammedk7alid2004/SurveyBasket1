namespace SurveyBasket.Contract.Contracts.Vote;

public record VoteAnswerRequest
(
  int QuestionId, 
  int AnswerId
);
