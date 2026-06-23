namespace GatherUp.Infrastructure.Security;
public class JwtSettings
{
    public required string SecretKey { get; set; }
    public string Issuer { get; set; } = "GatherUp.API";
    public string Audience { get; set; } = "GatherUp.Client";
    public int ExpiryMinutes { get; set; } = 120;
}
