using System.Collections.Generic;
using System.Linq;
using GatherUp.API.Dtos;
using GatherUp.API.Security;
using GatherUp.BL;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    private readonly PollService _pollService;
    private readonly EventDashboardService _dashboardService;

    public PollsController(PollService pollService, EventDashboardService dashboardService)
    {
        _pollService = pollService;
        _dashboardService = dashboardService;
    }

    private IActionResult? EnsureCallerManagesEvent(int eventId)
    {
        var ev = _dashboardService.GetEventDetails(eventId);
        if (ev == null) return NotFound(new { error = "האירוע לא נמצא." });
        if (ev.EventManagerId != User.GetUserId()) return Forbid();
        return null;
    }

    [Authorize]
    [HttpGet("event/{eventId}")]
    public IActionResult GetByEvent([FromRoute] int eventId)
    {
        return Ok(_pollService.GetEventPolls(eventId));
    }

    /// <summary>
    /// יצירת סקר - הגנת הקשר נקודתית: רק מנהל האירוע הספציפי הזה יכול ליצור
    /// בו סקרים.
    /// </summary>
    [Authorize]
    [HttpPost("event/{eventId}")]
    public IActionResult Create([FromRoute] int eventId, [FromBody] CreatePollRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;

        List<PollQuestion> questions = request.Questions
            .Select((q, index) => new PollQuestion
            {
                Id = index + 1,
                QuestionText = q.QuestionText,
                Options = q.Options
            })
            .ToList();

        var poll = _pollService.CreatePoll(eventId, request.Name, request.Description, request.ClosingDate, questions);
        return StatusCode(201, poll);
    }

    /// <summary>
    /// בדיקה כללית - האם הסקר עדיין פתוח להצבעה. פתוח לציבור, אין בה מידע רגיש.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{pollId}/is-open")]
    public IActionResult IsOpen([FromRoute] int pollId)
    {
        return Ok(new { isOpen = _pollService.IsPollOpen(pollId) });
    }

    /// <summary>
    /// הצבעה - מזהה המשתתף נלקח מהטוקן (לא מהבקשה), כדי שאי אפשר להצביע
    /// "בשם" משתתף אחר.
    /// </summary>
    [Authorize]
    [HttpPost("{pollId}/questions/{questionId}/vote")]
    public IActionResult Vote([FromRoute] int pollId, [FromRoute] int questionId, [FromBody] VoteRequest request)
    {
        _pollService.CastVote(pollId, questionId, User.GetUserId(), request.ChosenOption);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{pollId}/results")]
    public IActionResult GetResults([FromRoute] int pollId)
    {
        var (poll, results) = _pollService.GetPollWithResults(pollId);
        return Ok(new { poll, results });
    }
}
