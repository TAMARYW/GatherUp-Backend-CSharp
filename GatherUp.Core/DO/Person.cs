using System.Collections.Generic;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO;

/// <summary>
/// כל אדם שנרשם למערכת. זהות גלובלית - Id (ת.ז), שם, אימייל.
/// אין Role קבוע: תפקידו של אדם (מנהל / בעל אירוע / משתתף) נגזר
/// מהקשר האירוע (EventManagerId / EventHostId / Participants)
/// ולא ממחלקה נגזרת.
/// </summary>
public class Person : IEntity
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("Email")]
    public required string Email { get; set; }

    /// <summary>
    /// העדפות מייל — משותף לכל אדם במערכת ללא תלות בתפקידו באירוע.
    /// </summary>
    [XmlArray("NotificationPreferences")]
    [XmlArrayItem("Preference")]
    public List<NotificationType> NotificationPreferences { get; set; } = new();

    public Person() { }
}