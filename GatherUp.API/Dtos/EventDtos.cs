using System;

namespace GatherUp.API.Dtos;

/// <summary>
/// יצירת אירוע חדש. EventManagerId לא מתקבל מהלקוח - הוא נקבע בקונטרולר
/// מתוך מזהה המשתמש המחובר בטוקן, כדי שאי אפשר "ליצור אירוע בשם מנהל אחר".
/// </summary>
public record CreateEventRequest(string Name, int EventHostId, DateTime? Date, string? Location, decimal? PricePerParticipant);

/// <summary>
/// עריכת אירוע קיים. EventManagerId ו-EventHostId אינם ניתנים לשינוי אחרי
/// היצירה (פישוט מכוון - העברת בעלות על אירוע חורגת ממסגרת הפרויקט).
/// </summary>
public record UpdateEventRequest(string Name, DateTime? Date, string? Location, decimal? PricePerParticipant);
