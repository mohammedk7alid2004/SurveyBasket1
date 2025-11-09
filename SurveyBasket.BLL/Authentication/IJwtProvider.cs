namespace SurveyBasket.BLL.Authentication;

public interface IJwtProvider
{
    (string token, int ExpiresIn) GenerateToken(ApplicationUser user);
    string ? ValidateToken(string token);
}
