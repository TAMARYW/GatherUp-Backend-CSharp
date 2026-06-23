using System;

namespace GatherUp.API.Dtos;

public record CreateEventRequest(string Name, int EventHostId, DateTime? Date, string? Location, decimal? PricePerParticipant);

public record UpdateEventRequest(string Name, DateTime? Date, string? Location, decimal? PricePerParticipant);
