using System;
using System.Collections.Generic;

namespace GatherUp.API.Dtos;

public record PollQuestionRequest(string QuestionText, List<string> Options);

public record CreatePollRequest(string Name, string? Description, DateTime ClosingDate, List<PollQuestionRequest> Questions);

public record VoteRequest(string ChosenOption);
