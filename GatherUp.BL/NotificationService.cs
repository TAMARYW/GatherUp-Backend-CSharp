using System;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Events;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

/// <summary>
/// מאזינה לאירועי מערכת ושולחת מיילים למי שביקש לקבל עדכונים.
/// לא מבצעת שום לוגיקה עסקית — רק translate אירוע → מייל.
/// כעת בודקת NotificationPreferences ישירות על Person (ולא על EventManager/Participant).
/// </summary>
public class NotificationService
{
    private readonly IRepository<Person> _personRepo;
    private readonly IRepository<Event>  _eventRepo;
    private readonly IEmailService       _emailService;

    public NotificationService(
        IManagerNotificationEvents     managerEvents,
        IParticipantNotificationEvents participantEvents,
        IRepository<Person>            personRepo,
        IRepository<Event>             eventRepo,
        IEmailService                  emailService)
    {
        if (managerEvents     == null) throw new ArgumentNullException(nameof(managerEvents));
        if (participantEvents == null) throw new ArgumentNullException(nameof(participantEvents));
        _personRepo   = personRepo   ?? throw new ArgumentNullException(nameof(personRepo));
        _eventRepo    = eventRepo    ?? throw new ArgumentNullException(nameof(eventRepo));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        managerEvents.AttendanceConfirmed += OnAttendanceConfirmed;
        managerEvents.PaymentMade         += OnPaymentMade;
        managerEvents.PollAnswered        += OnPollAnswered;

        participantEvents.PollCreated          += OnPollCreated;
        participantEvents.EventDetailsChanged  += OnEventDetailsChanged;
    }

    private void OnAttendanceConfirmed(object? sender, AttendanceConfirmedEventArgs e) =>
        NotifyManager(e.EventManagerId, NotificationType.AttendanceConfirmed,
            $"משתתף (Id={e.ParticipantId}) {(e.IsAttending ? "אישר" : "ביטל")} הגעה לאירוע {e.EventId}.");

    private void OnPaymentMade(object? sender, PaymentMadeEventArgs e) =>
        NotifyManager(e.EventManagerId, NotificationType.PaymentMade,
            $"משתתף (Id={e.ParticipantId}) שילם ₪{e.AmountPaid} לאירוע {e.EventId}.");

    private void OnPollAnswered(object? sender, PollAnsweredEventArgs e) =>
        NotifyManager(e.EventManagerId, NotificationType.PollAnswered,
            $"משתתף (Id={e.ParticipantId}) הצביע '{e.ChosenOption}' בשאלה {e.QuestionId} בסקר {e.PollId}.");

    private void OnPollCreated(object? sender, PollCreatedEventArgs e) =>
        NotifyEventParticipants(e.EventId, NotificationType.PollCreated,
            $"נפתח סקר חדש: {e.PollName}.");

    private void OnEventDetailsChanged(object? sender, EventDetailsChangedEventArgs e) =>
        NotifyEventParticipants(e.EventId, NotificationType.EventDetailsChanged,
            $"פרטי האירוע {e.EventId} עודכנו.");

    /// <summary>
    /// שולחת מייל למנהל האירוע אם ביקש להיות מעודכן על סוג ההתראה הזה.
    /// </summary>
    private void NotifyManager(int managerId, NotificationType type, string message)
    {
        Person? manager = _personRepo.GetById(managerId);
        if (manager != null && manager.NotificationPreferences.Contains(type))
            _emailService.SendEmail(manager.Email, "עדכון מ-GatherUp", message);
    }

    /// <summary>
    /// שולחת מייל לכל משתתף שאישר הגעה וביקש להיות מעודכן.
    /// </summary>
    private void NotifyEventParticipants(int eventId, NotificationType type, string message)
    {
        Event? ev = _eventRepo.GetById(eventId);
        if (ev == null) return;

        ev.Participants
            .Where(p => p.IsAttending == true)
            .Select(p => _personRepo.GetById(p.PersonId))
            .Where(person => person != null && person.NotificationPreferences.Contains(type))
            .ToList()
            .ForEach(person => _emailService.SendEmail(person!.Email, "עדכון מ-GatherUp", message));
    }
}