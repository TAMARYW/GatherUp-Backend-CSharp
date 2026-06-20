using System;

namespace GatherUp.Core.Events;

public class AttendanceConfirmedEventArgs : EventArgs
{
    public int ParticipantId { get; }
    public bool IsAttending { get; }
    public int EventId { get; }
    public int EventManagerId { get; }

    public AttendanceConfirmedEventArgs(int participantId, bool isAttending, int eventId, int eventManagerId)
    {
        ParticipantId = participantId;
        IsAttending = isAttending;
        EventId = eventId;
        EventManagerId = eventManagerId;
    }
}

public class PaymentMadeEventArgs : EventArgs
{
    public int ParticipantId { get; }
    public decimal AmountPaid { get; }
    public int EventId { get; }
    public int EventManagerId { get; }

    public PaymentMadeEventArgs(int participantId, decimal amountPaid, int eventId, int eventManagerId)
    {
        ParticipantId = participantId;
        AmountPaid = amountPaid;
        EventId = eventId;
        EventManagerId = eventManagerId;
    }
}

public class PollAnsweredEventArgs : EventArgs
{
    public int PollId { get; }
    public int QuestionId { get; }
    public int ParticipantId { get; }
    public string ChosenOption { get; }
    public int EventId { get; }
    public int EventManagerId { get; }

    public PollAnsweredEventArgs(int pollId, int questionId, int participantId, string chosenOption, int eventId, int eventManagerId)
    {
        PollId = pollId;
        QuestionId = questionId;
        ParticipantId = participantId;
        ChosenOption = chosenOption;
        EventId = eventId;
        EventManagerId = eventManagerId;
    }
}

public class PollCreatedEventArgs : EventArgs
{
    public int PollId { get; }
    public int EventId { get; }
    public string PollName { get; }

    public PollCreatedEventArgs(int pollId, int eventId, string pollName)
    {
        PollId = pollId;
        EventId = eventId;
        PollName = pollName;
    }
}

public class EventDetailsChangedEventArgs : EventArgs
{
    public int EventId { get; }

    public EventDetailsChangedEventArgs(int eventId)
    {
        EventId = eventId;
    }
}