namespace SurveyBasket.Contract.Contracts;

public static class RegexPatterns
{
    public const string Password =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
}
