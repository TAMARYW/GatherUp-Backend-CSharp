namespace GatherUp.Core.Interfaces;

/// <summary>
/// חוזה ל"פרסום" אירועי מערכת. מחלקות הלוגיקה שמבצעות פעולה (אישור הגעה, תשלום,
/// הצבעה, יצירת סקר, עריכת אירוע) מקבלות ממשק זה בבנאי ומפעילות עליו Raise* -
/// בלי לדעת בכלל אם יש מאזינים, וכמה, ומה הם עושים עם המידע (זה כל הרעיון:
/// לא לשלוח מייל ישירות, אלא רק "להכריז" שמשהו קרה).
/// </summary>
public interface IEventPublisher
{
    void RaiseAttendanceConfirmed(int participantId, bool isAttending, int eventId, int eventManagerId);
    void RaisePaymentMade(int participantId, decimal amountPaid, int eventId, int eventManagerId);
    void RaisePollAnswered(int pollId, int questionId, int participantId, string chosenOption, int eventId, int eventManagerId);
    void RaisePollCreated(int pollId, int eventId, string pollName);
    void RaiseEventDetailsChanged(int eventId);
}