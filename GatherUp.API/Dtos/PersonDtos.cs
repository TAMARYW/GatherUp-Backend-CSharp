using System.Collections.Generic;
using GatherUp.Core.DO;

namespace GatherUp.API.Dtos;

public record RegisterPersonRequest(int Id, string Name, string Email);

public record UpdatePersonRequest(string Name, string Email);

public record NotificationPreferencesRequest(List<NotificationType> Preferences);