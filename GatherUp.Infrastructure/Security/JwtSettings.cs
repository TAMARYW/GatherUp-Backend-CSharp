namespace GatherUp.Infrastructure.Security;

/// <summary>
/// הגדרות הנפקת/אימות הטוקנים. הערכים בפועל מגיעים מ-appsettings.json בפרויקט
/// ה-API (פרק "Jwt"), ו-Program.cs בונה מהם מופע יחיד של המחלקה הזו ורושם אותו
/// כ-Singleton - כדי שגם JwtTokenService (בהנפקה) וגם הקונפיגורציה של
/// AddJwtBearer (באימות) ישתמשו באותו מפתח חתימה בדיוק.
/// </summary>
public class JwtSettings
{
    public required string SecretKey { get; set; }
    public string Issuer { get; set; } = "GatherUp.API";
    public string Audience { get; set; } = "GatherUp.Client";
    public int ExpiryMinutes { get; set; } = 120;
}
