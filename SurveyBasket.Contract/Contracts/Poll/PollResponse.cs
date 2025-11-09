namespace SurveyBasket.Contract.Contracts.Poll;
public record PollResponse
(
    int Id,
    string Title,
    string Description,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);

