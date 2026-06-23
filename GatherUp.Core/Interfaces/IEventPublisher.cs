namespace GatherUp.Core.Interfaces;

public interface IEventPublisher
{
    void RaiseAttendanceConfirmed(int participantId, bool isAttending, int eventId, int eventManagerId);
    void RaisePaymentMade(int participantId, decimal amountPaid, int eventId, int eventManagerId);
    void RaisePollAnswered(int pollId, int questionId, int participantId, string chosenOption, int eventId, int eventManagerId);
    void RaisePollCreated(int pollId, int eventId, string pollName);
    void RaiseEventDetailsChanged(int eventId);
}