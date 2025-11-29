namespace SurveyBasket.Contract.Contracts.Authentication;

public class ResetPasswordValidator:AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.code)
            .NotEmpty();
        RuleFor(x=>x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
    .WithMessage("Password must contain at least 8 characters, including upper case, lower case, number, and special character.");
    }
}
