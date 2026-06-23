namespace GatherUp.BL;

public record ParticipantWithDetails(
    int PersonId,
    string Name,
    string Email,
    bool? IsAttending,
    bool HasPaid,
    decimal AmountContributed);