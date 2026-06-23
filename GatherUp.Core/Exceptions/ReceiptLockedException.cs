using System;

namespace GatherUp.Core.Exceptions;

public class ReceiptLockedException : Exception
{
    public ReceiptLockedException(string message) : base(message)
    {
    }
}
