namespace SurveyBasket.Contract.Contracts.Authentication;

public class ForgetPasswordValidator:AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
