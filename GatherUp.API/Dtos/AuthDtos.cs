namespace GatherUp.API.Dtos;

public record LoginRequest(string Email, string IdCard);

public record LoginResponse(string Token, int Id, string Name);