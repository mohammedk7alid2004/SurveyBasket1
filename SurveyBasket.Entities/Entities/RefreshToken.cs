namespace SurveyBasket.Entities.Entities;
[Owned]
public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expireson { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }
    public bool IsExpired=> DateTime.UtcNow >= Expireson;
    public bool IsActive => RevokedOn == null && !IsExpired;
}
