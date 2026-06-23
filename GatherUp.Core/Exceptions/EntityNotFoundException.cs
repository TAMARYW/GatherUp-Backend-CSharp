using System;

namespace GatherUp.Core.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} עם מזהה {id} לא נמצא במערכת.")
    {
    }
}
