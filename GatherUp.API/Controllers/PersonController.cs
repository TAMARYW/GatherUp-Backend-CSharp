using GatherUp.API.Dtos;
using GatherUp.API.Security;
using GatherUp.BL;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly UserService _userService;

    public PersonController(UserService userService) => _userService = userService;

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Register([FromBody] RegisterPersonRequest request)
    {
        var newPerson = new Person { Id = request.Id, Name = request.Name, Email = request.Email };
        _userService.RegisterNewUser(newPerson);
        return StatusCode(201, newPerson);
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var person = _userService.GetUserById(id);
        if (person == null) return NotFound(new { error = "המשתמש לא נמצא." });
        return Ok(person);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdatePersonRequest request)
    {
        if (User.GetUserId() != id) return Forbid();

        var updated = _userService.UpdateUserDetails(id, request.Name, request.Email);
        return Ok(updated);
    }

    [Authorize]
    [HttpPut("{id}/notification-preferences")]
    public IActionResult UpdateNotificationPreferences(
        [FromRoute] int id,
        [FromBody] NotificationPreferencesRequest request)
    {
        if (User.GetUserId() != id) return Forbid();

        _userService.UpdateNotificationPreferences(id, request.Preferences);
        return NoContent();
    }

    [Authorize]
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string name)
    {
        return Ok(_userService.SearchByName(name));
    }
}