using System;
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
    public Person() { }
}