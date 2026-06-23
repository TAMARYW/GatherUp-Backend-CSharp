using System;
using GatherUp.Core.Events;

namespace GatherUp.Core.Interfaces;

public interface IParticipantNotificationEvents
{
    event EventHandler<PollCreatedEventArgs> PollCreated;
    event EventHandler<EventDetailsChangedEventArgs> EventDetailsChanged;
}
