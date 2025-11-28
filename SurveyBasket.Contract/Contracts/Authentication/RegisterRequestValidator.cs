
namespace SurveyBasket.Contract.Contracts.Authentication;

public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
    .NotEmpty().WithMessage("Password is required")
    .Matches(RegexPatterns.Password)
    .WithMessage("Password must contain at least 8 characters, including upper case, lower case, number, and special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3,100);
        RuleFor(x => x.LastName)
           .NotEmpty()
           .Length(3,100);

    }
}
