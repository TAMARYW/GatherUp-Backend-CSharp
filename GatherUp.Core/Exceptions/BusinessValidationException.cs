using System;

namespace GatherUp.Core.Exceptions;

/// <summary>
/// נזרק כאשר שכבת ה-BL מזהה שהפעולה המבוקשת אינה חוקית במצב הנוכחי של המערכת
/// או שהקלט שהתקבל אינו תקין מבחינה עסקית - לדוגמה: הצבעה באפשרות שלא קיימת
/// ברשימת האפשרויות של השאלה, או הרשמה עם כתובת אימייל שכבר תפוסה. זהו חריג
/// "לקוח" - ה-GlobalExceptionMiddleware ב-API מתפוס אותו ומתרגם אותו לסטטוס
/// 400 Bad Request.
/// </summary>
public class BusinessValidationException : Exception
{
    public BusinessValidationException(string message) : base(message)
    {
    }
}
