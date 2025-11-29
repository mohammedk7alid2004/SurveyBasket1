namespace SurveyBasket.Contract.Contracts.Users;

public class ChangePasswordValidator:AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x=>x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must contain at least 8 characters, including upper case, lower case, number, and special character.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("new password  must not equal current password");
            
    }
}
