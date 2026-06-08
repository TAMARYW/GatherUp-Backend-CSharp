using System;
using System.Collections.Generic;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Data;

public static class DataInitializer
{
    public static void Initialize(
        IRepository<Event> eventRepository,
        IRepository<EventManager> managerRepository,
        IRepository<EventHost> hostRepository,
        IRepository<Participant> participantRepository,
        IRepository<VendorAllocation> vendorRepository,
        IRepository<Poll> pollRepository)
    {
        var manager = new EventManager
        {
            Name = "תמר לוי",
            Email = "tamar.gatherup@gmail.com" 
        };
        managerRepository.Add(manager);

        var host = new EventHost
        {
            Name = "אביטל כהן",
            Email = "avital.gatherup@gmail.com" 
        };
        hostRepository.Add(host);

        var participant1 = new Participant
        {
            Name = "ישראל ישראלי",
            Email = "tamar.gatherup@gmail.com", 
            IsAttending = true,
            HasPaid = true,
            AmountContributed = 150.00m
        };
        participant1.MailingPreferences.Add("שינויי לו\"ז");
        participantRepository.Add(participant1);

        var participant2 = new Participant
        {
            Name = "שירה רפאל",
            Email = "avital.gatherup@gmail.com",
            IsAttending = null, // טרם השיבה
            HasPaid = false,
            AmountContributed = 0m
        };
        participant2.MailingPreferences.Add("סקרים חדשים");
        participantRepository.Add(participant2);

        var initialPoll = new Poll
        {
            Name = "סקר פרטים התחלתיים לאירוע",
            Description = "נא להצביע על התאריך והמיקום המועדפים עליכם"
        };
        
        var q1 = new PollQuestion 
        { 
            Id = 1, 
            QuestionText = "מהו המיקום המועדף עליכם?", 
            Options = new List<string> { "תל אביב", "ירושלים", "חיפה" } 
        };
        var q2 = new PollQuestion 
        { 
            Id = 2, 
            QuestionText = "באיזה תאריך נוח לכם?", 
            Options = new List<string> { "15/06/2026", "22/06/2026" } 
        };
        initialPoll.Questions.Add(q1);
        initialPoll.Questions.Add(q2);
        pollRepository.Add(initialPoll);

        var followUpPoll = new Poll
        {
            Name = "סקר המשך - כיבוד ואוכל",
            Description = "בחירת סגנון הקייטרינג לאירוע"
        };
        var q3 = new PollQuestion
        {
            Id = 1,
            QuestionText = "איזה סוג אוכל תרצו?",
            Options = new List<string> { "בשרי", "חלבי", "טבעוני" }
        };
        followUpPoll.Questions.Add(q3);
        pollRepository.Add(followUpPoll);

        var vendor = new VendorAllocation
        {
            Name = "קייטרינג אסאדו דלוקס",
            AmountOwed = 5000.00m, // חוב של 5000 ש"ח
            HasReceipt = true
        };
        vendor.Receipts.Add(new ReceiptDetails("REC-10024", 1500.00m, DateTime.Now.AddDays(-5)));
        vendorRepository.Add(vendor);

        var newEvent = new Event
        {
            Name = "ערב גיבוש צוות GatherUp",
            Date = null, 
            Location = null, 
            PricePerParticipant = 150.00m,
            EventManagerId = manager.Id, 
            EventHostId = host.Id        
        };
        
        newEvent.ParticipantIds.Add(participant1.Id);
        newEvent.ParticipantIds.Add(participant2.Id);
        newEvent.VendorIds.Add(vendor.Id);
        newEvent.PollIds.Add(initialPoll.Id);
        newEvent.PollIds.Add(followUpPoll.Id);

        eventRepository.Add(newEvent);
    }
}