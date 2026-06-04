using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace o_campista.api.Services
{

    public class TokenService
    {
        public string GenerateToken(string usuario)
        {
            var key = Encoding.UTF8.GetBytes(
                "SUPER_SECRET_KEY_O_CAMPISTA_2026"
            );

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, usuario)
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}