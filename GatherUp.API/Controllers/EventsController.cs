using GatherUp.API.Dtos;
using GatherUp.API.Security;
using GatherUp.BL;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventDashboardService _dashboardService;

    public EventsController(EventDashboardService dashboardService) =>
        _dashboardService = dashboardService;

    [Authorize]
    [HttpGet("organizer")]
    public IActionResult GetAsOrganizer() =>
        Ok(_dashboardService.GetEventsAsOrganizer(User.GetUserId()));

    [Authorize]
    [HttpGet("owner")]
    public IActionResult GetAsOwner() =>
        Ok(_dashboardService.GetEventsAsOwner(User.GetUserId()));

    [Authorize]
    [HttpGet("participant")]
    public IActionResult GetAsParticipant() =>
        Ok(_dashboardService.GetEventsAsParticipant(User.GetUserId()));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var ev = _dashboardService.GetEventDetails(id);
        if (ev == null) return NotFound(new { error = "האירוע לא נמצא." });
        return Ok(ev);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Create([FromBody] CreateEventRequest request)
    {
        var newEvent = new Event
        {
            Name = request.Name,
            EventManagerId = User.GetUserId(),
            EventHostId = request.EventHostId,
            Date = request.Date,
            Location = request.Location,
            PricePerParticipant = request.PricePerParticipant
        };

        _dashboardService.CreateEvent(newEvent);
        return StatusCode(201, newEvent);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateEventRequest request)
    {
        var ev = _dashboardService.GetEventDetails(id);
        if (ev == null) return NotFound(new { error = "האירוע לא נמצא." });
        if (ev.EventManagerId != User.GetUserId()) return Forbid();

        ev.Name = request.Name;
        ev.Date = request.Date;
        ev.Location = request.Location;
        ev.PricePerParticipant = request.PricePerParticipant;

        _dashboardService.UpdateEventDetails(ev);
        return Ok(ev);
    }
}