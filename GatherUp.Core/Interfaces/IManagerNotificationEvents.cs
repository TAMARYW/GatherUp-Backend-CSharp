using System;
using GatherUp.Core.Events;

namespace GatherUp.Core.Interfaces;

public interface IManagerNotificationEvents
{
    event EventHandler<AttendanceConfirmedEventArgs> AttendanceConfirmed;
    event EventHandler<PaymentMadeEventArgs> PaymentMade;
    event EventHandler<PollAnsweredEventArgs> PollAnswered;
}
