using GatherUp.API.Dtos;
using GatherUp.BL;
using GatherUp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService    _userService;
    private readonly ITokenService  _tokenService;

    public AuthController(UserService userService, ITokenService tokenService)
    {
        _userService  = userService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// כניסה — אימייל + ת.ז. מצליח → JWT (ללא Role גלובלי).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var person = _userService.AuthenticateUser(request.Email, request.IdCard);
        if (person == null)
            return Unauthorized(new { error = "אימייל או תעודת זהות שגויים." });

        string token = _tokenService.GenerateToken(person);
        return Ok(new LoginResponse(token, person.Id, person.Name));
    }
}