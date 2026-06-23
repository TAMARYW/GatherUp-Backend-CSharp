using System.Xml.Serialization;

namespace GatherUp.Core.DO;

public class Participant
{
    [XmlAttribute("PersonId")]
    public int PersonId { get; set; }

    [XmlElement("IsAttending")]
    public bool? IsAttending { get; set; }

    [XmlAttribute("HasPaid")]
    public bool HasPaid { get; set; }

    [XmlElement("AmountContributed")]
    public decimal AmountContributed { get; set; }

    public Participant() { }
}