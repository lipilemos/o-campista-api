using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace o_campista.api.Services
{

    public class TokenService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(
            "SUPER_SECRET_KEY_O_CAMPISTA_2026"
        );

        public string GenerateToken(string usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateResetToken(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("purpose", "password-reset")
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string? ValidateResetToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(
                    token, parameters, out _);

                var purpose = principal.FindFirst("purpose")?.Value;
                if (purpose != "password-reset")
                    return null;

                return principal.FindFirst(ClaimTypes.Email)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}