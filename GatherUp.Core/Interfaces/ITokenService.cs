using GatherUp.Core.DO;

namespace GatherUp.Core.Interfaces;

/// <summary>
/// חוזה להנפקת טוקן מאובטח עבור משתמש שעבר אימות בהצלחה (email + ת.ז במסך
/// הכניסה). ה-AuthController בשכבת ה-API תלוי בממשק הזה בלבד - הוא לא יודע
/// ולא צריך לדעת שמדובר ב-JWT, מה החתימה, או מה תוקף הטוקן. המימוש בפועל
/// (JwtTokenService) נמצא בשכבת ה-Infrastructure, באותו הרוח של IEmailService
/// ו-IRepository: ה-BL/API "מכריזים" מה הם צריכים, וה-Infrastructure מספק.
/// </summary>
public interface ITokenService
{
    string GenerateToken(Person person);
}
