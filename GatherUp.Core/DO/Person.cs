using System.Collections.Generic;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO;

public class Person : IEntity
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("Email")]
    public required string Email { get; set; }

    [XmlArray("NotificationPreferences")]
    [XmlArrayItem("Preference")]
    public List<NotificationType> NotificationPreferences { get; set; } = new();

    public Person() { }
}