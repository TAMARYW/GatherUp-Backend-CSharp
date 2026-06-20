namespace GatherUp.API.Dtos;

public record LoginRequest(string Email, string IdCard);

/// <summary>
/// תגובת כניסה — אין Role גלובלי; תפקיד האדם נקבע לפי הקשר האירוע.
/// </summary>
public record LoginResponse(string Token, int Id, string Name);