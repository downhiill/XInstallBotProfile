using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Context;
using XInstallBotProfile.Generate;
using XInstallBotProfile.Models;

namespace XInstallBotProfile.Controllers
{
   /// <summary>
   /// 
   /// </summary>
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

            if (model.Password != user.PasswordHash)
            {
                return Unauthorized("Неверный логин или пароль.");
            }


            // Генерация нового access токена с UserId и Role
            var newAccessToken = TokenGenerator.GenerateAccessToken(user.Login, user.Id, user.Role);

            // Генерация нового refresh токена
            var newRefreshTokenToUpdate = TokenGenerator.GenerateRefreshToken();

            // Обновляем refresh токен в базе данных
            user.JwtToken = newRefreshTokenToUpdate;

            // Сохраняем изменения в БД
            await _context.SaveChangesAsync();

            // Устанавливаем refresh token в HttpOnly cookie
            Response.Cookies.Append("refreshToken", newRefreshTokenToUpdate, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // Возвращаем access token + id + роль в ответе
            return Ok(new
            {
                AccessToken = newAccessToken,
                User = new User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Role = user.Role,
                    Nickname = user.Nickname,
                    IsDsp = user.IsDsp,
                    IsDspInApp = user.IsDspInApp,
                    IsDspBanner = user.IsDspBanner
                }
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Получаем refresh-токен из куки
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh-токен отсутствует.");
            }

            try
            {
                // Находим пользователя с этим refresh-токеном
                var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == refreshToken);

                if (user == null)
                {
                    return Unauthorized("Неверный refresh-токен.");
                }

                // Удаляем refresh-токен из базы данных, чтобы нельзя было его использовать для обновления токенов
                user.JwtToken = null;

                // Сохраняем изменения в базе данных
                await _context.SaveChangesAsync();

                // Очищаем куки с refresh-токеном на стороне клиента
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true, // Доступен только через HTTP (нельзя прочитать через JS)
                    Secure = true, // Только для HTTPS
                    SameSite = SameSiteMode.Strict // Используется только в рамках одного сайта
                });

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
        public async Task<IActionResult> RefreshToken()
        {
            // Получаем refresh-токен из кук
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh-токен отсутствует.");
            }

            // Ищем пользователя по refresh-токену
            var user = await _context.Users.FirstOrDefaultAsync(u => u.JwtToken == refreshToken);

            if (user == null)
            {
                return Unauthorized("Неверный refresh-токен.");
            }

            // Генерируем новые токены
            var newAccessToken = TokenGenerator.GenerateAccessToken(user.Login, user.Id, user.Role);
            var newRefreshToken = TokenGenerator.GenerateRefreshToken();

            // Обновляем refresh-токен в БД
            user.JwtToken = newRefreshToken;
            await _context.SaveChangesAsync();

            // Устанавливаем новый refresh-токен в куки
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true, // Доступен только через HTTP (нельзя прочитать через JS)
                Secure = true, // Только по HTTPS
                SameSite = SameSiteMode.None, // Используется только в рамках одного сайта
                Expires = DateTime.UtcNow.AddDays(7) // Срок жизни куки
            });

            

            return Ok(new
            {
                AccessToken = newAccessToken,
                User = new User
                {
                    Id = user.Id,
                    Login = user.Login,
                    Role = user.Role,
                    Nickname = user.Nickname,
                    IsDsp = user.IsDsp,
                    IsDspInApp = user.IsDspInApp,
                    IsDspBanner = user.IsDspBanner
                }
            });
        }

    }
}
