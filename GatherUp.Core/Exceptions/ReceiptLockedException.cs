using System;

namespace GatherUp.Core.Exceptions;

/// <summary>
/// נזרק כאשר מתבצע ניסיון לעדכן או למחוק קבלה פיננסית קיימת. קבלות הן רשומות
/// "נעולות" (Append-Only) במערכת - לאחר שנוצרו ונשמרו, אסור לשנות אותן או
/// למחוק אותן, מאחר שהן מהוות תיעוד חשבונאי מחייב. ה-GlobalExceptionMiddleware
/// ב-API מתפוס אותו ומתרגם אותו לסטטוס 400 Bad Request.
/// </summary>
public class ReceiptLockedException : Exception
{
    public ReceiptLockedException(string message) : base(message)
    {
    }
}
