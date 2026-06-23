namespace GatherUp.API.Dtos;

public record AddVendorDebtRequest(string Name, decimal Amount);

public record PaymentRequest(decimal Amount);

public record PaymentReminderRequest(string Message);