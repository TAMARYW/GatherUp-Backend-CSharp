using System;
using GatherUp.Core.Events;

namespace GatherUp.Core.Interfaces;

/// <summary>
/// אירועים שמנהל האירוע (EventManager) עשוי לרצות להירשם אליהם, כדי לקבל מייל
/// כשמשהו מהם קורה - בלי שמחלקת הלוגיקה שמפעילה את הפעולה תדע בכלל שמייל נשלח.
/// </summary>
public interface IManagerNotificationEvents
{
    event EventHandler<AttendanceConfirmedEventArgs> AttendanceConfirmed;
    event EventHandler<PaymentMadeEventArgs> PaymentMade;
    event EventHandler<PollAnsweredEventArgs> PollAnswered;
}
