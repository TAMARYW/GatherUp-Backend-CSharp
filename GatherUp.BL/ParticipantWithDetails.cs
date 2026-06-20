namespace GatherUp.BL;

/// <summary>
/// תוצאת join בין Person ל-Participant — מאחד את פרטי הזהות (Name, Email)
/// עם מצב ההשתתפות (IsAttending, HasPaid, AmountContributed) עבור תצוגה
/// ב-API ובלקוח. נוצר בשכבת ה-BL לפי בקשה ואינו נשמר.
/// </summary>
public record ParticipantWithDetails(
    int PersonId,
    string Name,
    string Email,
    bool? IsAttending,
    bool HasPaid,
    decimal AmountContributed);