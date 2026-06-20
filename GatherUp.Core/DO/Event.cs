using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO;

public class Event : IEntity
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("Date")]
    public DateTime? Date { get; set; }

    [XmlElement("Location")]
    public string? Location { get; set; }

    [XmlElement("PricePerParticipant")]
    public decimal? PricePerParticipant { get; set; }

    /// <summary>Person.Id של מנהל האירוע.</summary>
    [XmlElement("EventManagerId")]
    public required int EventManagerId { get; set; }

    /// <summary>Person.Id של בעל האירוע (חייב להיות קיים ב-Person.xml).</summary>
    [XmlElement("EventHostId")]
    public required int EventHostId { get; set; }

    /// <summary>
    /// המשתתפים לאירוע זה — מוכלים ישירות בתוך Event.xml.
    /// כל Participant שומר PersonId (הפניה ל-Person.xml) ומצב פר-אירוע.
    /// </summary>
    [XmlArray("Participants")]
    [XmlArrayItem("Participant")]
    public List<Participant> Participants { get; set; }

    [XmlArray("VendorIds")]
    [XmlArrayItem("VendorId")]
    public List<int> VendorIds { get; set; }

    [XmlArray("PollIds")]
    [XmlArrayItem("PollId")]
    public List<int> PollIds { get; set; }

    [SetsRequiredMembers]
    public Event()
    {
        Participants = new List<Participant>();
        VendorIds = new List<int>();
        PollIds = new List<int>();
    }
}