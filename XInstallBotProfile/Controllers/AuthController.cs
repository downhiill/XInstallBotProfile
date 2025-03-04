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
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Service.AdminPanelService.Models.Request.LoginRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            // Если у пользователя нет refresh token, генерируем новый
            if (string.IsNullOrEmpty(user.JwtToken))
            {
                var newRefreshToken = TokenGenerator.GenerateRefreshToken();
                user.JwtToken = newRefreshToken; // Сохраняем новый refresh token
            }

            // Генерация нового access токена
            var newAccessToken = TokenGenerator.GenerateAccessToken(user.Login);

            // Генерация нового refresh токена (если нужно обновить)
            var newRefreshTokenToUpdate = TokenGenerator.GenerateRefreshToken();

            // Обновляем refresh токен в базе данных (если требуется)
            user.JwtToken = newRefreshTokenToUpdate;

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            // Устанавливаем refresh token в HttpOnly cookie
            Response.Cookies.Append("refreshToken", newRefreshTokenToUpdate, new CookieOptions
            {
                HttpOnly = true,   // Запрещает доступ к cookie через JavaScript
                Secure = true,     // Cookie будет отправляться только по HTTPS
                SameSite = SameSiteMode.Strict, // Защищает от межсайтовых запросов
                Expires = DateTime.UtcNow.AddDays(7) // Устанавливаем срок действия cookie (например, 7 дней)
            });

            // Возвращаем access token в ответе
            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }





        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            // Проверка на наличие refresh-токена в запросе
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token не может быть пустым.");
            }

            try
            {
                // Найдем пользователя с этим refresh-токеном
                var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == request.RefreshToken);

                if (user == null)
                {
                    return BadRequest("Некорректный refresh-токен.");
                }

                // Удаляем refresh-токен (чтобы нельзя было обновить access-токен)
                user.JwtToken = null;

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();

                return Ok(new { message = "Вы успешно вышли из системы." });
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отладки
                _logger.LogError("Ошибка при выходе: {0}", ex.Message);
                return StatusCode(500, "Ошибка на сервере.");
            }
        }
    


        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken(HttpContext httpContext)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == httpContext.Request.Cookies["refreshToken"]);

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
