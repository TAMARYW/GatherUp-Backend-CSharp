using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

public class ParticipantVote
{
    [XmlAttribute("ParticipantId")]
    public int ParticipantId { get; set; }

    [XmlAttribute("ChosenOption")]
    public string ChosenOption { get; set; } = string.Empty;
}

public class PollQuestion
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlElement("QuestionText")]
    public required string QuestionText { get; set; }

    [XmlArray("Options")]
    [XmlArrayItem("Option")]
    public List<string> Options { get; set; }

    [XmlArray("Votes")]
    [XmlArrayItem("Vote")]
    public List<ParticipantVote> Votes { get; set; }

    [SetsRequiredMembers]
    public PollQuestion()
    {
        Options = new List<string>();
        Votes = new List<ParticipantVote>();
    }
}