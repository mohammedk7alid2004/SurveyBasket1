namespace SurveyBasket.BLL.Settings;

public class MailSettings
{
    public string Email { get; set; } = string.Empty;
    public string AppPassword { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool SSL { get; set; }
    public bool IsBodyHtml { get; set; }
}
