namespace GatherUp.Core.Interfaces;

/// <summary>
/// חוזה מופשט לשליחת מייל. ה-BL לא יודע ולא צריך לדעת איך זה ממומש בפועל
/// (כתיבה לקובץ, שרת SMTP אמיתי, או כל דבר אחר) - הוא רק קורא לזה.
/// </summary>
public interface IEmailService
{
    void SendEmail(string toAddress, string subject, string body);
}
