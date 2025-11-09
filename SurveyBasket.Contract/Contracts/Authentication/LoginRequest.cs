namespace SurveyBasket.Contract.Contracts.Authentication;

public record LoginRequest
(
    string Email,
    string Password
);