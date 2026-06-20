namespace GatherUp.Core.DO;

/// <summary>
/// סוגי ההתראות שמשתמש (מנהל או משתתף) יכול לבחור אם לקבל עליהן מייל.
/// שלוש הראשונות רלוונטיות בעיקר למנהל, שתי האחרונות בעיקר למשתתף -
/// אבל מאחר שהשדה משותף ל-Person, אין מניעה טכנית שמשתמש יבחר גם קטגוריה
/// ש"שייכת" לתפקיד אחר.
/// </summary>
public enum NotificationType
{
    AttendanceConfirmed,
    PaymentMade,
    PollAnswered,
    PollCreated,
    EventDetailsChanged
}
