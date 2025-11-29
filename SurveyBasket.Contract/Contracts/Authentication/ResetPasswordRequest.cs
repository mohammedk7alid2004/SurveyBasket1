namespace SurveyBasket.Contract.Contracts.Authentication;

public record ResetPasswordRequest
(
    string email,
    string code,
    string NewPassword
);
