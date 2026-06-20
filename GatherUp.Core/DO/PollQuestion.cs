using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GatherUp.Core.DO;

/// <summary>
/// הצבעה בודדת של משתתף לשאלה. נדרש כמחלקה נפרדת (ולא Dictionary&lt;int,string&gt;) כי
/// XmlSerializer לא יודע לסריאלז Dictionary כלל - זו הסיבה שב-PollQuestion המקורי השדה
/// היה מסומן [XmlIgnore], מה שגרם לכל הצבעה להיעלם בכל פעם שקוראים מחדש מה-XML.
/// </summary>
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

    /// <summary>
    /// תוקן: היה Dictionary&lt;int,string&gt; מסומן [XmlIgnore] - כלומר ההצבעות מעולם לא
    /// נשמרו בפועל בדיסק, ונעלמו בכל קריאה חדשה. כעת רשימה רגילה של ParticipantVote,
    /// שמסתדרת מצוין עם XmlSerializer.
    /// </summary>
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