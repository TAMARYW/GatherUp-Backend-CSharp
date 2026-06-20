using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.BL;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Notifications;
using GatherUp.Infrastructure.Repositories;

namespace GatherUp.TestProject;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== GatherUp - הרצת זרימה מלאה מקצה לקצה ===\n");

        if (!Directory.Exists("XML")) Directory.CreateDirectory("XML");

        // -----------------------------------------------------------------
        // 1. הקמת הריפוזיטורי המוחשי מבוסס ה-XML (היחיד שמכיר את זה הוא Main)
        // -----------------------------------------------------------------
        IRepository<Person> personRepo = new XmlRepository<Person>(Path.Combine("XML", "Person.xml"), useSerializer: true);
        IRepository<Event> eventRepo = new XmlRepository<Event>(Path.Combine("XML", "Event.xml"), useSerializer: true);
        IRepository<VendorAllocation> vendorRepo = new XmlRepository<VendorAllocation>(Path.Combine("XML", "VendorAllocation.xml"), useSerializer: true);
        IRepository<Poll> pollRepo = new XmlRepository<Poll>(Path.Combine("XML", "Poll.xml"), useSerializer: true);
        // נוסף בשלב 4: ריפוזיטורי הקבלות, שהיה קיים בקוד אך לא חובר אף פעם בפועל.
        IReceiptRepository receiptRepo = new ReceiptRepository();

        // נתוני דמו - רק בהרצה הראשונה (אם ה-XML עוד ריק)
        if (!eventRepo.GetAll().Any())
        {
            Console.WriteLine("מאתחלת נתוני דמו ל-XML...\n");
            DataInitializer.Initialize(eventRepo, personRepo, vendorRepo, pollRepo);
        }

        // -----------------------------------------------------------------
        // 2. מיד לאחר הקמת הריפוזיטורי - מקימים את שירותי הלוגיקה ומזריקים להם
        //    אותו, וכן מקימים מופע אחד ומשותף של "תחנת השידור" (EventNotifier)
        //    שמשמש גם כ-IEventPublisher (למי שמכריז) וגם כשני ממשקי ההאזנה
        //    (למי שמאזין) - בלי ששני הצדדים מכירים זה את זה.
        // -----------------------------------------------------------------
        var eventNotifier = new EventNotifier();
        IEmailService emailService = new FileEmailService(Path.Combine("Emails", "outbox.txt"));

        var userService = new UserService(personRepo);
        var dashboardService = new EventDashboardService(eventRepo, personRepo, eventNotifier);
        var participantService = new ParticipantService(personRepo, eventRepo, emailService, eventNotifier);
        var financeService = new FinanceService(vendorRepo, personRepo, eventRepo, receiptRepo, emailService, eventNotifier);
        var pollService = new PollService(pollRepo, eventRepo, eventNotifier);

        // NotificationService לא נקרא ע"י אף אחד ישירות - הוא רק נרשם לאירועים בבנאי שלו,
        // ומגיב להם באופן עצמאי לאורך כל ריצת התוכנית.
        var notificationService = new NotificationService(eventNotifier, eventNotifier, personRepo, emailService);

        // -----------------------------------------------------------------
        // 3. סימולציית תהליך מלא, כאילו מגיע מלחיצות על המסכים שתכננו
        // -----------------------------------------------------------------

        // [מסך כניסה] המנהלת מתחברת עם המייל והת.ז שלה
        Event seedEvent = eventRepo.GetAll().First();
        Person? managerAsPerson = personRepo.GetById(seedEvent.EventManagerId);
        if (managerAsPerson == null) throw new InvalidOperationException("לא נמצא מנהל לאירוע הדמו.");

        Person? loggedInManager = userService.AuthenticateUser(managerAsPerson.Email, managerAsPerson.Id.ToString());
        Console.WriteLine(loggedInManager != null
            ? $"[מסך כניסה] התחברות הצליחה: {loggedInManager.Name} ({loggedInManager.Role})"
            : "[מסך כניסה] התחברות נכשלה");

        // [מסך הבית] הצגת כל האירועים שהמשתמש מארגן
        Console.WriteLine("\n[מסך הבית] אירועים בהם המשתמש הוא המארגן:");
        dashboardService.GetEventsAsOrganizer(managerAsPerson.Id).ToList()
            .ForEach(e => Console.WriteLine($"  - {e.Name} (Id={e.Id})"));

        // [לחיצה על האירוע -> טאב משתתפים] הצגת כל המשתתפים, כולל סטטוס תשלום והגעה
        Console.WriteLine("\n[מסך פרטי אירוע > משתתפים] רשימת משתתפים:");
        List<Participant> participants = participantService.GetEventParticipants(seedEvent.Id).ToList();
        participants.ForEach(p => Console.WriteLine(
            $"  - {p.Name}: הגעה={p.IsAttending}, שולם={p.HasPaid}, תרומה={p.AmountContributed}₪"));

        Participant firstParticipant = participants[0];
        Participant secondParticipant = participants[1];

        // [מסך המשתתף] לחיצה על "אישור הגעה" - לשני המשתתפים, כדי ש"ערוצי ההתראות"
        // שבחרו במעמד הרישום (NotificationPreferences) "יופעלו" בהמשך הסימולציה
        Console.WriteLine($"\n[מסך משתתף - {firstParticipant.Name}] לוחצת על 'אישור הגעה'...");
        participantService.ConfirmAttendance(firstParticipant.Id, true);
        Console.WriteLine($"[מסך משתתף - {secondParticipant.Name}] לוחצת על 'אישור הגעה'...");
        participantService.ConfirmAttendance(secondParticipant.Id, true);
        // מאחורי הקלעים: כל קריאה הפעילה RaiseAttendanceConfirmed, שגרם ל-
        // NotificationService לבדוק מי מהמנהלים ביקש לדעת על כך, ולשלוח לו מייל (לקובץ).

        // [מסך מנהל > כספים] לוחצת על 'שלח תזכורת תשלום' - שירה עדיין לא שילמה בשלב הזה
        Console.WriteLine("\n[מסך מנהל - כספים] לוחצת על 'שלח תזכורת תשלום'...");
        financeService.SendPaymentReminders(seedEvent.Id, "בנק הפועלים, חשבון 123-456789");

        // [מסך המשתתף] לחיצה על "שלם" - שירה מקבלת את התזכורת ומשלמת בתגובה
        Console.WriteLine($"\n[מסך משתתף - {secondParticipant.Name}] משלמת {seedEvent.PricePerParticipant}₪...");
        financeService.RegisterPayment(secondParticipant.Id, seedEvent.PricePerParticipant ?? 0);

        // [מסך מנהל > כספים] בדיקת התקציב הדינמי לאחר התשלום
        decimal netBudget = financeService.CalculateNetBudget(seedEvent.Id);
        Console.WriteLine($"\n[מסך מנהל - כספים] תקציב נטו נוכחי לאחר התשלום: {netBudget}₪");

        AccountSummaryResult summary = financeService.GetAccountSummary(seedEvent.Id);
        Console.WriteLine($"[מסך מנהל - כספים] סיכום חשבון: הכנסות={summary.TotalIncome}₪, הוצאות={summary.TotalOutgoing}₪, מאזן={summary.NetBalance}₪");

        // [מסך מנהל > סקרים] יצירת סקר חדש
        Console.WriteLine("\n[מסך מנהל - סקרים] לוחצת על 'סקר חדש' ומגדירה שאלה...");
        var newPoll = pollService.CreatePoll(
            seedEvent.Id,
            "סקר שעת התחלה",
            "באיזו שעה נוח לכולם שהאירוע יתחיל?",
            DateTime.Now.AddDays(3),
            new List<PollQuestion>
            {
                new PollQuestion
                {
                    Id = 1,
                    QuestionText = "מה שעת ההתחלה המועדפת?",
                    Options = new List<string> { "18:00", "19:30", "21:00" }
                }
            });
        // מאחורי הקלעים: CreatePoll הפעילה RaisePollCreated, וה-NotificationService שלח
        // מייל לכל המשתתפים שביקשו לדעת על סקרים חדשים (participant2 בנתוני הדמו).

        // [מסך משתתף > סקרים] המשתתפת מצביעה בסקר
        Console.WriteLine($"[מסך משתתף - {secondParticipant.Name}] מצביעה '19:30' בסקר...");
        pollService.CastVote(newPoll.Id, 1, secondParticipant.Id, "19:30");

        // המשתתפת מתחרטת ומצביעה שוב - בודקים שזה מעדכן ולא משכפל
        pollService.CastVote(newPoll.Id, 1, secondParticipant.Id, "21:00");

        bool isOpen = pollService.IsPollOpen(newPoll.Id);
        Console.WriteLine($"[מסך מנהל - סקרים] הסקר {(isOpen ? "עדיין פתוח" : "סגור")} להצבעה.");

        var (poll, results) = pollService.GetPollWithResults(newPoll.Id);
        Console.WriteLine($"[מסך מנהל - סקרים] תוצאות הסקר '{poll.Name}' באחוזים:");
        foreach (var questionResult in results)
        {
            foreach (var optionPercent in questionResult.Value)
            {
                Console.WriteLine($"  - אופציה '{optionPercent.Key}': {optionPercent.Value}%");
            }
        }

        // [מסך מנהל > פרטי אירוע] עריכת פרטי האירוע
        Console.WriteLine("\n[מסך מנהל - פרטי אירוע] משנה את המיקום ושומרת...");
        seedEvent.Location = "אולמי הגן הירוק, ראשון לציון";
        dashboardService.UpdateEventDetails(seedEvent);
        // מאחורי הקלעים: UpdateEventDetails הפעילה RaiseEventDetailsChanged, וה-
        // NotificationService שלח מייל לכל המשתתפים שביקשו לדעת על שינוי באירוע
        // (participant1 בנתוני הדמו).

        // -----------------------------------------------------------------
        // 4. אימות מקצה לקצה: גישה ישירה לקבצי ה-XML הפיזיים בדיסק, לבדוק
        //    שהשינויים שביצעה שכבת הלוגיקה אכן חלחלו ונשמרו, בלי שה-BL בכלל
        //    יודע שקובץ XML קיים.
        // -----------------------------------------------------------------
        Console.WriteLine("\n=== אימות ישיר על קובצי XML שעל הדיסק ===");
        PrintFileSnippet(Path.Combine("XML", "Person.xml"));
        PrintFileSnippet(Path.Combine("XML", "Event.xml"));
        PrintFileSnippet(Path.Combine("XML", "Poll.xml"));

        Console.WriteLine("\n=== תוכן תיבת הדואר היוצא (Emails/outbox.txt) ===");
        PrintFileSnippet(Path.Combine("Emails", "outbox.txt"));

        Console.WriteLine("\n=== סיום הריצה ===");
    }

    static void PrintFileSnippet(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"({path} עוד לא נוצר)");
            return;
        }

        Console.WriteLine($"--- {path} ---");
        Console.WriteLine(File.ReadAllText(path));
    }
}
