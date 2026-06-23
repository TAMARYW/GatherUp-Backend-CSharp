using GatherUp.API.Dtos;
using GatherUp.API.Security;
using GatherUp.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly ParticipantService _participantService;
    private readonly EventDashboardService _dashboardService;

    public ParticipantsController(ParticipantService participantService, EventDashboardService dashboardService)
    {
        _participantService = participantService;
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
    public IActionResult GetByEvent([FromRoute] int eventId) =>
        Ok(_participantService.GetEventParticipants(eventId));

    [Authorize]
    [HttpPost("event/{eventId}")]
    public IActionResult Add([FromRoute] int eventId, [FromBody] AddParticipantRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;

        var result = _participantService.AddParticipant(eventId, request.Email);
        return StatusCode(201, result);
    }

    [Authorize]
    [HttpPut("event/{eventId}/attendance")]
    public IActionResult ConfirmAttendance([FromRoute] int eventId, [FromBody] AttendanceRequest request)
    {
        _participantService.ConfirmAttendance(eventId, User.GetUserId(), request.IsAttending);
        return NoContent();
    }

    [Authorize]
    [HttpGet("event/{eventId}/unconfirmed")]
    public IActionResult GetUnconfirmed([FromRoute] int eventId)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;
        return Ok(_participantService.GetUnconfirmedParticipants(eventId));
    }

    [Authorize]
    [HttpPost("event/{eventId}/invitations")]
    public IActionResult SendInvitations([FromRoute] int eventId, [FromBody] MassInvitationRequest request)
    {
        var forbidResult = EnsureCallerManagesEvent(eventId);
        if (forbidResult != null) return forbidResult;
        _participantService.SendMassInvitations(eventId, request.ConfirmationFormLink);
        return NoContent();
    }
}