namespace SurveyBasket.Contract.Contracts.Authentication;

public class RefreshTokenRequestValidator:AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
            RuleFor(x=>x.Token)
                .NotEmpty().WithMessage("Token is required.");
            RuleFor(x=>x.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token is required.");
    }
}
