namespace SurveyBasket.Contract.Contracts.Email;

public class EmailConfrimValidation:AbstractValidator<EmailConfrimRequest>
{
    public EmailConfrimValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.Code)
            .NotEmpty();
    }
}

