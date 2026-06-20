namespace GatherUp.API.Dtos;

/// <summary>
/// הוספת/רישום חוב לספק. ה-Id מגיע מה-route (vendorId); כאן רק שם הספק
/// (נדרש רק אם זו רישום ספק חדש - מתעלם ממנו אם הספק כבר קיים) והסכום.
/// </summary>
public record AddVendorDebtRequest(string Name, decimal Amount);

public record PaymentRequest(decimal Amount);

/// <summary>
/// תזכורת תשלום - תוכן חופשי שהמנהל קובע (לא רק פרטי בנק).
/// </summary>
public record PaymentReminderRequest(string Message);