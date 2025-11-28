namespace SurveyBasket.Contract.Contracts.Email;

public record EmailConfrimRequest
(
    string UserId,
    string Code
);
