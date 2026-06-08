using System;

namespace GatherUp.Core.DO;

public record ReceiptDetails(string ReceiptNumber, decimal Amount, DateTime Date, string? SavedFilePath = null);