namespace Topsis.Adapters.Encryption;

public class RecaptchaSettings
{
    public string ProjectId { get; set; }
    public string SiteKey { get; set; } = null!;
    public string SecretKey { get; internal set; }
}
