﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace XInstallBotProfile.Generate
{
    public class TokenGenerator
    {
        private static string _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

        public static string GenerateAccessToken(string username, int userId, string role)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // ID пользователя
                new Claim(ClaimTypes.Role, role) // Роль пользователя
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
            return Convert.ToBase64String(randomBytes);
        }
    }
}
