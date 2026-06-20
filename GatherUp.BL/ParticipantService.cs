using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL;

/// <summary>
/// ניהול משתתפים — הוספה לפי אימייל, אישור הגעה, הפצת הזמנות.
/// המשתתפים מוכלים בתוך Event.Participants ונשמרים כחלק מה-Event.
/// </summary>
public class ParticipantService
{
    private readonly IRepository<Person> _personRepo;
    private readonly IRepository<Event>  _eventRepo;
    private readonly IEmailService       _emailService;
    private readonly IEventPublisher     _eventPublisher;

    public ParticipantService(
        IRepository<Person> personRepo,
        IRepository<Event>  eventRepo,
        IEmailService       emailService,
        IEventPublisher     eventPublisher)
    {
        _personRepo     = personRepo     ?? throw new ArgumentNullException(nameof(personRepo));
        _eventRepo      = eventRepo      ?? throw new ArgumentNullException(nameof(eventRepo));
        _emailService   = emailService   ?? throw new ArgumentNullException(nameof(emailService));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    private Event GetExistingEvent(int eventId)
    {
        Event? ev = _eventRepo.GetById(eventId);
        if (ev == null) throw new EntityNotFoundException("האירוע", eventId);
        return ev;
    }

    /// <summary>
    /// הוספת משתתף לאירוע לפי אימייל.
    /// האדם חייב להיות קיים ב-Person.xml; אי אפשר להזמין מישהו שלא נרשם למערכת.
    /// </summary>
    public ParticipantWithDetails AddParticipant(int eventId, string email)
    {
        Event ev = GetExistingEvent(eventId);

        Person? person = _personRepo.GetAll()
            .FirstOrDefault(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));

        if (person == null)
            throw new BusinessValidationException(
                $"לא נמצא אדם עם האימייל '{email}' במערכת. יש להירשם תחילה.");

        if (ev.Participants.Any(p => p.PersonId == person.Id))
            throw new BusinessValidationException("אדם זה כבר רשום כמשתתף באירוע.");

        var participant = new Participant { PersonId = person.Id };
        ev.Participants.Add(participant);
        _eventRepo.Update(ev);

        return ToDetails(participant, person);
    }

    /// <summary>
    /// כל המשתתפים של אירוע — מוחזרים עם פרטי הזהות מ-Person.xml.
    /// </summary>
    public IEnumerable<ParticipantWithDetails> GetEventParticipants(int eventId)
    {
        Event ev = GetExistingEvent(eventId);
        return ev.Participants.Select(p => ToDetails(p, _personRepo.GetById(p.PersonId)));
    }

    /// <summary>
    /// אישור / דחיית הגעה — האדם מאשר על עצמו (personId == User.GetUserId()).
    /// המנהל אינו מאשר הגעה של אחרים.
    /// </summary>
    public void ConfirmAttendance(int eventId, int personId, bool isAttending)
    {
        Event ev = GetExistingEvent(eventId);

        Participant? participant = ev.Participants.FirstOrDefault(p => p.PersonId == personId);
        if (participant == null) throw new EntityNotFoundException("המשתתף", personId);

        participant.IsAttending = isAttending;
        _eventRepo.Update(ev);

        _eventPublisher.RaiseAttendanceConfirmed(personId, isAttending, ev.Id, ev.EventManagerId);
    }

    /// <summary>
    /// משתתפים שטרם אישרו הגעה — לתצוגה ולשליחת הזמנות המוניות.
    /// </summary>
    public IEnumerable<ParticipantWithDetails> GetUnconfirmedParticipants(int eventId)
    {
        return GetEventParticipants(eventId).Where(p => p.IsAttending != true);
    }

    /// <summary>
    /// הפצת הזמנות המוניות: שולחת מייל לכל מי שטרם אישר הגעה.
    /// </summary>
    public void SendMassInvitations(int eventId, string confirmationFormLink)
    {
        Event ev = GetExistingEvent(eventId);

        ev.Participants
            .Where(p => p.IsAttending != true)
            .Select(p => _personRepo.GetById(p.PersonId))
            .Where(person => person != null)
            .ToList()
            .ForEach(person => _emailService.SendEmail(
                person!.Email,
                "אישור הגעה לאירוע",
                $"שלום {person.Name}, נא לאשר הגעה לאירוע בקישור הבא: {confirmationFormLink}"));
    }

    private static ParticipantWithDetails ToDetails(Participant p, Person? person) =>
        new(p.PersonId,
            person?.Name  ?? "לא ידוע",
            person?.Email ?? "",
            p.IsAttending,
            p.HasPaid,
            p.AmountContributed);
}