using System;

namespace GatherUp.Core.Exceptions;

/// <summary>
/// נזרק כאשר שכבת ה-BL מתבקשת לאתר רשומה (אירוע, משתתף, סקר, ספק, משתמש וכו')
/// לפי מזהה שאינו קיים במערכת. זהו חריג "לקוח" לכל דבר - הבקשה עצמה הייתה תקינה
/// תחבירית, אבל המשאב המבוקש לא קיים. ה-GlobalExceptionMiddleware ב-API מתפוס
/// אותו ומתרגם אותו לסטטוס 404 Not Found.
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// בנאי נוח שבונה הודעה עקבית מתוך שם הישות והמזהה המבוקש, כדי שלא נצטרך
    /// לכתוב את אותה מחרוזת "X לא נמצא" שוב ושוב בכל מקום בקוד ה-BL.
    /// </summary>
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} עם מזהה {id} לא נמצא במערכת.")
    {
    }
}
