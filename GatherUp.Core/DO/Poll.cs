using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO;

public class Poll : IEntity
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("Description")]
    public string? Description { get; set; }

    /// <summary>
    /// תאריך סגירת הסקר להצבעה. נחשף בלבד דרך PollService.IsPollOpen - לא חוסם
    /// הצבעה אוטומטית, ההחלטה איך להתייחס לסקר סגור נשארת בידי המסך.
    /// </summary>
    [XmlElement("ClosingDate")]
    public DateTime? ClosingDate { get; set; }

    [XmlArray("Questions")]
    [XmlArrayItem("PollQuestion")]
    public List<PollQuestion> Questions { get; set; }

    [SetsRequiredMembers]
    public Poll()
    {
        Questions = new List<PollQuestion>();
    }
}
