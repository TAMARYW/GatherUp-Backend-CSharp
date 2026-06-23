using System;

namespace GatherUp.Core.Exceptions;

public class BusinessValidationException : Exception
{
    public BusinessValidationException(string message) : base(message)
    {
    }
}
