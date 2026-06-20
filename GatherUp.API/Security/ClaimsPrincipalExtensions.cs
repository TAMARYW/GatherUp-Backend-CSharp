using System;
using System.Security.Claims;

namespace GatherUp.API.Security;

/// <summary>
/// מתודת הרחבה ל-ClaimsPrincipal (ה-User בתוך כל קונטרולר) - מחלצת את מזהה
/// המשתמש המחובר מתוך הטוקן. נדרשת בכל מקום שבו אסור להסתמך על מזהה שמגיע
/// מהלקוח (כמו participantId בגוף הבקשה), אלא חובה לקחת אותו מהטוקן החתום -
/// כדי שמשתמש לא יוכל "להתחזות" למשתמש אחר.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        string? idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null || !int.TryParse(idClaim, out int id))
            throw new InvalidOperationException("לא נמצא מזהה משתמש תקין בטוקן.");

        return id;
    }

    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        string? email = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("לא נמצאה כתובת אימייל תקינה בטוקן.");

        return email;
    }
}
