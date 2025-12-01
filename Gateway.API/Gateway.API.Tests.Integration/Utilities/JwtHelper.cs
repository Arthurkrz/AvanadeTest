using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.API.Tests.Integration.Utilities
{
    public static class JwtHelper
    {
        public static string CreateTestJwt(string role, string issuer, string audience, string key)
        {
            var handler = new JwtSecurityTokenHandler();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = handler.CreateJwtSecurityToken(
                issuer: issuer,
                audience: audience,
                subject: new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, role)
                }),
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256)
            );

            return handler.WriteToken(token);
        }
    }
}
