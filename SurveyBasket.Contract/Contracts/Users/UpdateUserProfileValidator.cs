
namespace SurveyBasket.Contract.Contracts.Users;

public class UpdateUserProfileValidator:AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);
    }
}
