using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

public class PollQuestion
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("QuestionText")]
    public required string QuestionText { get; set; }

    [XmlArray("Options")]
    [XmlArrayItem("Option")]
    public List<string> Options { get; set; }

    [XmlIgnore]
    public Dictionary<int, string> ParticipantVotes { get; set; }

    [SetsRequiredMembers]
    public PollQuestion()
    {
        Options = new List<string>();
        ParticipantVotes = new Dictionary<int, string>();
    }
}