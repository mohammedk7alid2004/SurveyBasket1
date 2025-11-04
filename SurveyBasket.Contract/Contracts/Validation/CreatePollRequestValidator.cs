using SurveyBasket.Contract.Contracts.Request;
namespace SurveyBasket.Contract.Contracts.Validation;

public class CreatePollRequestValidator:AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(3,100);
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .Length(5,500);
    }
}
