namespace SurveyBasket.Contract.Contracts.Email;

public class ResendConfrimatioEmailValidation:AbstractValidator<ResendConfrimatioEmailRequest>
{
    public ResendConfrimatioEmailValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
