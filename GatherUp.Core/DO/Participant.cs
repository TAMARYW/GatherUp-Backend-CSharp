using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

public class Participant : Person
{
    [XmlElement("IsAttending")]
    public bool? IsAttending { get; set; }

    [XmlAttribute("HasPaid")]
    public bool HasPaid { get; set; }

    [XmlElement("AmountContributed")]
    public decimal AmountContributed { get; set; }

    [XmlArray("MailingPreferences")]
    [XmlArrayItem("Preference")]
    public List<string> MailingPreferences { get; set; }

    [SetsRequiredMembers]
    public Participant() : base()
    {
        MailingPreferences = new List<string>();
    }
}