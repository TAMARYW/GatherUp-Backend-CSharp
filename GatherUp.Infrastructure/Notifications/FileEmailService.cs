using System;
using System.IO;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Notifications;

/// <summary>
/// מימוש פשוט של IEmailService: "שולח" מייל ע"י כתיבת שורה לקובץ טקסט
/// (כתובת, נושא והודעה) - מספיק כדי להדגים את כל הזרימה בלי שרת מייל אמיתי.
/// (אתגר הבונוס בתרגיל הוא להחליף את זה במימוש שכן שולח מייל אמיתי.)
/// </summary>
public class FileEmailService : IEmailService
{
    private readonly string _outboxFilePath;

    public FileEmailService(string outboxFilePath = "Emails/outbox.txt")
    {
        _outboxFilePath = outboxFilePath;
    }

    public void SendEmail(string toAddress, string subject, string body)
    {
        string? directory = Path.GetDirectoryName(_outboxFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] To: {toAddress} | Subject: {subject} | Body: {body}{Environment.NewLine}";
        File.AppendAllText(_outboxFilePath, entry);
    }
}
