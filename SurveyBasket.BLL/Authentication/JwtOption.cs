
namespace SurveyBasket.BLL.Authentication;

public class JwtOption
{
    public static string SectionName = "Jwt";
    [Required]
    public string Key { get; init; }=string.Empty;
    [Required]
    public string Issuer { get; init; }=string.Empty;
    [Required]
    public string Audience { get; init; }=string.Empty;
    [Range(1,int.MaxValue)]
    public int ExpireMinutes { get; init; }
}
