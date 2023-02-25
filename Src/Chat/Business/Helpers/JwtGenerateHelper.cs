using CommonDomainLayer.Configurations;
using DomainData.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Helpers;

public class JwtGenerateHelper
{
    public string GenerateAccessToken(User user, List<Claim> claims,
        JwtConfiguration jwtConfiguration)
    {
        var token = new JwtSecurityToken
        (
            issuer: jwtConfiguration.ValidIssuer,
            audience: jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtConfiguration.JwtExpireMinutes),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(jwtConfiguration.IssuerSigningKey)),
                    SecurityAlgorithms.HmacSha512)
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public string GenerateRefreshToken(User user, List<Claim> claims,
        JwtConfiguration jwtConfiguration)
    {
        var token = new JwtSecurityToken
        (
            issuer: jwtConfiguration.ValidIssuer,
            audience: jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddDays(jwtConfiguration.JwtRefreshExpireDays),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(jwtConfiguration.IssuerSigningKey)),
                    SecurityAlgorithms.HmacSha512)
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}
