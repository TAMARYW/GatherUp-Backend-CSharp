namespace GatherUp.API.Dtos;

public record AddParticipantRequest(string Email);

public record AttendanceRequest(bool IsAttending);

public record MassInvitationRequest(string ConfirmationFormLink);