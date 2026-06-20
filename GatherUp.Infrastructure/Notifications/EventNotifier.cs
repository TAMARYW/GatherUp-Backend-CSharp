using System;
using GatherUp.Core.Events;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Notifications;

/// <summary>
/// "תחנת השידור" המשותפת לכל אירועי המערכת. מחלקות הלוגיקה שמבצעות פעולה מקבלות
/// אותה כ-IEventPublisher כדי "להכריז" שמשהו קרה, ו-NotificationService (ב-BL) מקבל
/// אותה כ-IManagerNotificationEvents / IParticipantNotificationEvents כדי להאזין -
/// שני הצדדים לא מכירים זה את זה בכלל, רק את האובייקט המשותף הזה.
/// </summary>
public class EventNotifier : IEventPublisher, IManagerNotificationEvents, IParticipantNotificationEvents
{
    public event EventHandler<AttendanceConfirmedEventArgs>? AttendanceConfirmed;
    public event EventHandler<PaymentMadeEventArgs>? PaymentMade;
    public event EventHandler<PollAnsweredEventArgs>? PollAnswered;
    public event EventHandler<PollCreatedEventArgs>? PollCreated;
    public event EventHandler<EventDetailsChangedEventArgs>? EventDetailsChanged;

    public void RaiseAttendanceConfirmed(int participantId, bool isAttending, int eventId, int eventManagerId)
        => AttendanceConfirmed?.Invoke(this, new AttendanceConfirmedEventArgs(participantId, isAttending, eventId, eventManagerId));

    public void RaisePaymentMade(int participantId, decimal amountPaid, int eventId, int eventManagerId)
        => PaymentMade?.Invoke(this, new PaymentMadeEventArgs(participantId, amountPaid, eventId, eventManagerId));

    public void RaisePollAnswered(int pollId, int questionId, int participantId, string chosenOption, int eventId, int eventManagerId)
        => PollAnswered?.Invoke(this, new PollAnsweredEventArgs(pollId, questionId, participantId, chosenOption, eventId, eventManagerId));

    public void RaisePollCreated(int pollId, int eventId, string pollName)
        => PollCreated?.Invoke(this, new PollCreatedEventArgs(pollId, eventId, pollName));

    public void RaiseEventDetailsChanged(int eventId)
        => EventDetailsChanged?.Invoke(this, new EventDetailsChangedEventArgs(eventId));
}