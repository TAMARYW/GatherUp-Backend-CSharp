using System;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

public record ReceiptDetails
{
    [XmlAttribute("ReceiptNumber")]
    public string ReceiptNumber { get; init; }

    [XmlElement("Amount")]
    public decimal Amount { get; init; }

    [XmlElement("Date")]
    public DateTime Date { get; init; }

    [XmlElement("SavedFilePath")]
    public string? SavedFilePath { get; init; }

    public ReceiptDetails(string receiptNumber, decimal amount, DateTime date, string? savedFilePath = null)
    {
        ReceiptNumber = receiptNumber;
        Amount = amount;
        Date = date;
        SavedFilePath = savedFilePath;
    }

    public ReceiptDetails() : this(string.Empty, 0m, default)
    {
    }
}
