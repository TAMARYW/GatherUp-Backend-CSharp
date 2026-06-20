namespace GatherUp.Infrastructure.Notifications;

/// <summary>
/// הגדרות שרת ה-SMTP לשליחת מיילים אמיתיים. הערכים בפועל מגיעים מ-appsettings.json
/// (פרק "Smtp"), באותו הרוח של JwtSettings - הפרטים הרגישים (App Password) לא
/// יושבים בקובץ הזה עצמו, אלא מוזרקים בזמן ריצה ממקור מאובטח (User Secrets / משתני סביבה).
/// </summary>
public class SmtpSettings
{
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public required string SenderEmail { get; set; }
    public string SenderName { get; set; } = "GatherUp";
    public required string AppPassword { get; set; }
    public bool EnableSsl { get; set; } = true;
}