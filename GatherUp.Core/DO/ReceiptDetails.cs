using System;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

/// <summary>
/// תוקן בשלב 4: נוסף בנאי ריק (פרמטרי) בנוסף לבנאי ה-record המקורי, ותויגו
/// השדות ב-Xml attributes - כדי ש-XmlSerializer יוכל לבנות ולקרוא מופעים של
/// הטיפוס הזה כשהוא משובץ בתוך VendorAllocation.Receipts (ראו את התיקון
/// המקביל ב-VendorAllocation.cs, שמסיר את [XmlIgnore] שגרם לקבלות "להיעלם"
/// בכל קריאה חדשה מה-XML - בדיוק אותו באג שתואר ותוקן בעבר ב-PollQuestion.Votes).
/// </summary>
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

    /// <summary>
    /// בנאי ריק הנדרש ע"י XmlSerializer בלבד (לא לשימוש ישיר בקוד שלנו).
    /// </summary>
    public ReceiptDetails() : this(string.Empty, 0m, default)
    {
    }
}
