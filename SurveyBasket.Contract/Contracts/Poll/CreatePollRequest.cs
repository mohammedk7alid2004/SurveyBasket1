namespace SurveyBasket.Contract.Contracts.Poll;
public record CreatePollRequest
(
    string Title,
    string Description,
    DateOnly StartsAt,
    DateOnly EndsAt
);