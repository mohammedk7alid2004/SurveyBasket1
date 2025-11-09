namespace SurveyBasket.Contract.Contracts.Poll;
public record UpdatePollRequest
(
    string Title,
    string Description,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);