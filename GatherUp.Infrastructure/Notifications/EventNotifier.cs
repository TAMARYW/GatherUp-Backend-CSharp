using System;
using GatherUp.Core.Events;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Notifications;

public class EventNotifier : IEventPublisher, IManagerNotificationEvents, IParticipantNotificationEvents
{
    public event EventHandler<AttendanceConfirmedEventArgs>? AttendanceConfirmed;
    public event EventHandler<PaymentMadeEventArgs>? PaymentMade;
    public event EventHandler<PollAnsweredEventArgs>? PollAnswered;
    public event EventHandler<PollCreatedEventArgs>? PollCreated;
    public event EventHandler<EventDetailsChangedEventArgs>? EventDetailsChanged;

    public void RaiseAttendanceConfirmed(int participantId, bool isAttending, int eventId, int eventManagerId)
        => AttendanceConfirmed?.Invoke(this, new AttendanceConfirmedEventArgs
        {
            ParticipantId = participantId,
            IsAttending = isAttending,
            EventId = eventId,
            EventManagerId = eventManagerId
        });

    public void RaisePaymentMade(int participantId, decimal amountPaid, int eventId, int eventManagerId)
        => PaymentMade?.Invoke(this, new PaymentMadeEventArgs
        {
            ParticipantId = participantId,
            AmountPaid = amountPaid,
            EventId = eventId,
            EventManagerId = eventManagerId
        });

    public void RaisePollAnswered(int pollId, int questionId, int participantId, string chosenOption, int eventId, int eventManagerId)
        => PollAnswered?.Invoke(this, new PollAnsweredEventArgs
        {
            PollId = pollId,
            QuestionId = questionId,
            ParticipantId = participantId,
            ChosenOption = chosenOption,
            EventId = eventId,
            EventManagerId = eventManagerId
        });

    public void RaisePollCreated(int pollId, int eventId, string pollName)
        => PollCreated?.Invoke(this, new PollCreatedEventArgs
        {
            PollId = pollId,
            EventId = eventId,
            PollName = pollName
        });

    public void RaiseEventDetailsChanged(int eventId)
        => EventDetailsChanged?.Invoke(this, new EventDetailsChangedEventArgs
        {
            EventId = eventId
        });
}