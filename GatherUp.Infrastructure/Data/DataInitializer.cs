using System;
using System.Collections.Generic;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Data;

/// <summary>
/// אתחול נתוני דמו.
/// Person.xml — זהויות בלבד.
/// Event.xml  — כל אירוע מכיל את רשימת ה-Participants שלו (PersonId + מצב).
/// </summary>
public static class DataInitializer
{
    public static void Initialize(
        IRepository<Person>          personRepo,
        IRepository<Event>           eventRepo,
        IRepository<VendorAllocation> vendorRepo,
        IRepository<Poll>            pollRepo)
    {
        // ── אנשים ──────────────────────────────────────────────────────
        var tamar = new Person
        {
            Name  = "תמר לוי",
            Email = "ty0556736234@gmail.com",
            NotificationPreferences = new List<NotificationType>
            {
                NotificationType.AttendanceConfirmed,
                NotificationType.PaymentMade,
                NotificationType.PollAnswered
            }
        };
        var avital = new Person { Name = "אביטל כהן",    Email = "avital.gatherup@gmail.com" };
        var israel = new Person
        {
            Name  = "ישראל ישראלי",
            Email = "israel.gatherup@gmail.com",
            NotificationPreferences = new List<NotificationType> { NotificationType.EventDetailsChanged }
        };
        var shira = new Person
        {
            Name  = "שירה רפאל",
            Email = "shira.gatherup@gmail.com",
            NotificationPreferences = new List<NotificationType> { NotificationType.PollCreated }
        };

        personRepo.Add(tamar);
        personRepo.Add(avital);
        personRepo.Add(israel);
        personRepo.Add(shira);

        // ── ספקים ─────────────────────────────────────────────────────
        var vendor1 = new VendorAllocation { Name = "קייטרינג אבי", AmountOwed = 2000 };
        vendorRepo.Add(vendor1);

        // ── סקרים ─────────────────────────────────────────────────────
        var poll1 = new Poll
        {
            Name        = "פרטים ראשוניים",
            Description = "בחירת תאריך ומיקום לאירוע",
            ClosingDate = DateTime.Now.AddDays(7),
        };
        poll1.Questions.Add(new PollQuestion
        {
            Id           = 1,
            QuestionText = "מתי מתאים לכם?",
            Options      = new List<string> { "יום שישי", "מוצאי שבת", "ראשון בערב" }
        });

        var poll2 = new Poll
        {
            Name        = "בחירת מיקום",
            ClosingDate = DateTime.Now.AddDays(14),
        };
        poll2.Questions.Add(new PollQuestion
        {
            Id           = 1,
            QuestionText = "איפה להתכנס?",
            Options      = new List<string> { "תל אביב", "ירושלים", "ראשון לציון" }
        });

        pollRepo.Add(poll1);
        pollRepo.Add(poll2);

        // ── אירועים עם משתתפים מוכלים ────────────────────────────────
        // אירוע 1: תמר מנהלת, אביטל בעלת אירוע, ישראל ושירה משתתפים
        var event1 = new Event
        {
            Name                = "ערב גיבוש צוות GatherUp",
            Location            = "אולמי הגן הירוק, ראשון לציון",
            PricePerParticipant = 150,
            EventManagerId      = tamar.Id,
            EventHostId         = avital.Id,
        };
        event1.Participants.Add(new Participant
        {
            PersonId          = israel.Id,
            IsAttending       = true,
            HasPaid           = true,
            AmountContributed = 300
        });
        event1.Participants.Add(new Participant
        {
            PersonId          = shira.Id,
            IsAttending       = true,
            HasPaid           = true,
            AmountContributed = 150
        });
        // תמר גם מנהלת אירוע זה וגם משתתפת — כשני "כובעים" מאותה Person
        event1.Participants.Add(new Participant
        {
            PersonId          = tamar.Id,
            IsAttending       = true,
            HasPaid           = false,
            AmountContributed = 0
        });
        event1.VendorIds.Add(vendor1.Id);
        event1.PollIds.Add(poll1.Id);
        event1.PollIds.Add(poll2.Id);
        eventRepo.Add(event1);

        // אירוע 2: ישראל מנהל, תמר בעלת אירוע, שירה משתתפת
        // מדגים: אדם אחד (ישראל) מנהל אירוע זה ומשתתף באירוע 1
        var event2 = new Event
        {
            Name                = "ארוחת צהריים עסקית",
            Date                = DateTime.Now.AddDays(30),
            Location            = "מסעדת הגורמה, תל אביב",
            PricePerParticipant = 120,
            EventManagerId      = israel.Id,
            EventHostId         = tamar.Id,
        };
        event2.Participants.Add(new Participant
        {
            PersonId    = shira.Id,
            IsAttending = null,  // טרם השיבה
        });
        eventRepo.Add(event2);
    }
}