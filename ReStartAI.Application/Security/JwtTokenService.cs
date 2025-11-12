using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ReStartAI.Application.Security;

public class JwtTokenService
{
    private readonly IConfiguration _cfg;

    public JwtTokenService(IConfiguration cfg)
    {
        _cfg = cfg;
    }

    public (string token, DateTime expiresAtUtc) CreateToken(string subject, IEnumerable<Claim>? extraClaims = null)
    {
        var issuer = _cfg["Jwt:Issuer"]!;
        var audience = _cfg["Jwt:Audience"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresMinutes"] ?? "60"));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (extraClaims != null) claims.AddRange(extraClaims);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }
}