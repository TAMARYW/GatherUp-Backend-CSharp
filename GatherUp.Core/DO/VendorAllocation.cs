using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO;

public class VendorAllocation : IEntity
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("AmountOwed")]
    public decimal AmountOwed { get; set; }

    [XmlAttribute("HasReceipt")]
    public bool HasReceipt { get; set; }

    /// <summary>
    /// תוקן בשלב 4: היה מסומן [XmlIgnore] - כלומר הקבלות שנוספו לספק נעלמו בכל
    /// פעם שה-VendorAllocation נקרא מחדש מה-XML (כל קריאה ל-GetAll/GetById
    /// קוראת מהדיסק מחדש). בדיוק אותו באג שתואר ותוקן בעבר ב-PollQuestion.Votes.
    /// כעת, בעזרת התיקון המקביל ב-ReceiptDetails (בנאי ריק + Xml attributes),
    /// הרשימה נשמרת ונטענת כרגיל.
    /// </summary>
    [XmlArray("Receipts")]
    [XmlArrayItem("Receipt")]
    public List<ReceiptDetails> Receipts { get; set; }

    [SetsRequiredMembers]
    public VendorAllocation()
    {
        Receipts = new List<ReceiptDetails>();
    }
}
