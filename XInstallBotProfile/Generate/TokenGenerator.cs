using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace XInstallBotProfile.Generate
{
    public class TokenGenerator
    {
        private static string _secretKey = "s2hG93b0qy32xvwp1PqX0M1aO9lmU4cT";

        public static string GenerateJwtToken(string username)
        {
            // Проверка на null для _secretKey
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = "yourIssuer",
                    ValidAudience = "yourAudience"
                };

                tokenHandler.ValidateToken(token, parameters, out var validatedToken);
                return true; // Токен валиден
            }
            catch
            {
                return false; // Токен невалиден
            }
        }
    }

}
