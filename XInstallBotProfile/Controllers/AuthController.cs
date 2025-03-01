using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Context;
using XInstallBotProfile.Generate;
using XInstallBotProfile.Models;

namespace XInstallBotProfile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Email);

            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            // Генерируем новые токены
            var newAccessToken = TokenGenerator.GenerateAccessToken(user.Login);
            var newRefreshToken = TokenGenerator.GenerateRefreshToken();

            // Обновляем Refresh Token в базе (перезаписываем pre-login токен)
            user.JwtToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == request.RefreshToken);

            if (user == null)
            {
                return BadRequest("Некорректный refresh-токен.");
            }

            // Удаляем refresh-токен (чтобы нельзя было обновить access-токен)
            user.JwtToken = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Вы успешно вышли из системы." });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == request.RefreshToken);

            if (user == null)
            {
                return Unauthorized("Неверный refresh-токен.");
            }

            // Генерируем новый Access Token
            var newAccessToken = TokenGenerator.GenerateAccessToken(user.Login);
            var newRefreshToken = TokenGenerator.GenerateRefreshToken();

            // Обновляем refresh-токен в базе
            user.JwtToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
