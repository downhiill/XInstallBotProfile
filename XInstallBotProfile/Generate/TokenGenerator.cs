using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace XInstallBotProfile.Generate
{
    public class TokenGenerator
    {
        private static string _secretKey = "s2hG93b0qy32xvwp1PqX0M1aO9lmU4cT";

        public static string GenerateAccessToken(string username)
        {
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
                expires: DateTime.UtcNow.AddHours(1), // Access-токен живет 1 час
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes); // Длинная случайная строка
        }
    }
}
