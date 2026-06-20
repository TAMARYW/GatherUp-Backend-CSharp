using System;
using GatherUp.Core.Events;

namespace GatherUp.Core.Interfaces;

/// <summary>
/// אירועים שמשתתף עשוי לרצות להירשם אליהם, כדי לקבל מייל כשמשהו מהם קורה.
/// </summary>
public interface IParticipantNotificationEvents
{
    event EventHandler<PollCreatedEventArgs> PollCreated;
    event EventHandler<EventDetailsChangedEventArgs> EventDetailsChanged;
}
