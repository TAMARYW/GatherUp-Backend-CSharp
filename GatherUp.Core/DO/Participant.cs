using System.Xml.Serialization;

namespace GatherUp.Core.DO;

/// <summary>
/// מייצג את ה-קשר בין אדם (PersonId) לאירוע ספציפי —
/// IsAttending, HasPaid, AmountContributed הם מצב פר-אירוע ולא
/// תכונות של האדם עצמו.
/// Participant אינו נשמר ב-Person.xml; הוא מוכל בתוך Event
/// (בשדה Participants) ונשמר כחלק מה-Event.xml.
/// </summary>
public class Participant
{
    /// <summary>מזהה האדם ב-Person.xml (ת.ז).</summary>
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