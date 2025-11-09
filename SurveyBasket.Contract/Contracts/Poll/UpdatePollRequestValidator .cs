namespace SurveyBasket.Contract.Contracts.Poll;

public class UpdatePollRequestValidator : AbstractValidator<UpdatePollRequest>
{
    public UpdatePollRequestValidator()
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
