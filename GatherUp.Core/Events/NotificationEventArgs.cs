using System;

namespace GatherUp.Core.Events;

public class AttendanceConfirmedEventArgs : EventArgs
{
    public int ParticipantId { get; init; }
    public bool IsAttending { get; init; }
    public int EventId { get; init; }
    public int EventManagerId { get; init; }
}

public class PaymentMadeEventArgs : EventArgs
{
    public int ParticipantId { get; init; }
    public decimal AmountPaid { get; init; }
    public int EventId { get; init; }
    public int EventManagerId { get; init; }
}

public class PollAnsweredEventArgs : EventArgs
{
    public int PollId { get; init; }
    public int QuestionId { get; init; }
    public int ParticipantId { get; init; }
    public string ChosenOption { get; init; } = string.Empty;
    public int EventId { get; init; }
    public int EventManagerId { get; init; }
}

public class PollCreatedEventArgs : EventArgs
{
    public int PollId { get; init; }
    public int EventId { get; init; }
    public string PollName { get; init; } = string.Empty;
}

public class EventDetailsChangedEventArgs : EventArgs
{
    public int EventId { get; init; }
}