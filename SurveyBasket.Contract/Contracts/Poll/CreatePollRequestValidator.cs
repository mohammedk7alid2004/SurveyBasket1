namespace SurveyBasket.Contract.Contracts.Poll;

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


        RuleFor(x => x.StartsAt)
            .NotEmpty().WithMessage("StartsAt is required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("StartsAt must be today or later");

        RuleFor(x => x.EndsAt)
            .NotEmpty().WithMessage("EndsAt is required")
            .GreaterThanOrEqualTo(x => x.StartsAt)
            .WithMessage("EndsAt must be after StartsAt");

    }
}
