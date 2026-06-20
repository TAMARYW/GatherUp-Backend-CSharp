namespace GatherUp.API.Dtos;

/// <summary>
/// הוספת משתתף לפי אימייל — האדם חייב להיות רשום ב-Person.xml.
/// </summary>
public record AddParticipantRequest(string Email);

public record AttendanceRequest(bool IsAttending);

public record MassInvitationRequest(string ConfirmationFormLink);