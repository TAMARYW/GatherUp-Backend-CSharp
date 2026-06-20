using System;
using System.Collections.Generic;

namespace GatherUp.API.Dtos;

public record PollQuestionRequest(string QuestionText, List<string> Options);

public record CreatePollRequest(string Name, string? Description, DateTime ClosingDate, List<PollQuestionRequest> Questions);

/// <summary>
/// הצבעה - participantId לא מתקבל בבקשה בכלל. הוא נלקח מהטוקן בקונטרולר,
/// כדי שמשתתף לא יוכל "להצביע בשם" מישהו אחר.
/// </summary>
public record VoteRequest(string ChosenOption);
