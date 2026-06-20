using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GatherUp.Infrastructure.Security;

/// <summary>
/// מנפיק JWT. Claims: Id, Name, Email בלבד.
/// אין Role גלובלי — הרשאות נגזרות מהקשר האירוע בלבד.
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(JwtSettings settings) =>
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

    public string GenerateToken(Person person)
    {
        if (person == null) throw new ArgumentNullException(nameof(person));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
            new Claim(ClaimTypes.Name,           person.Name),
            new Claim(ClaimTypes.Email,          person.Email),
        };

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _settings.Issuer,
            audience:           _settings.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}