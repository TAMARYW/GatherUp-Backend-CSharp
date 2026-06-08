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

    [XmlIgnore]
    public List<ReceiptDetails> Receipts { get; set; }

    [SetsRequiredMembers]
    public VendorAllocation()
    {
        Receipts = new List<ReceiptDetails>();
    }
}