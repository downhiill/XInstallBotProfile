using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Context;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;

namespace XInstallBotProfile.Controllers
{
    [Authorize(Roles = "Admin")] // Доступ только для админа
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Получение списка пользователей
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Nickname,
                    u.CreatedAt,
                    u.IsDsp,
                    u.IsDspInApp,
                    u.IsDspBanner
                })
                .ToListAsync();

            return Ok(users);
        }

        // 2. Обновление флагов пользователя
        [HttpPut("user/{id}/flags")]
        public async Task<IActionResult> UpdateUserFlags(int id, [FromBody] UpdateFlagsRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Пользователь не найден");

            user.IsDsp = request.Flag1;
            user.IsDspInApp = request.Flag2;
            user.IsDspBanner = request.Flag3;

            await _context.SaveChangesAsync();
            return Ok("Флаги обновлены");
        }

        // 3. Удаление пользователя
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Пользователь не найден");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Пользователь удалён");
        }

        // 4. Добавление пользователя (по умолчанию ник пустой)
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // Проверяем, занят ли никнейм, если он передан
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                bool usernameExists = await _context.Users.AnyAsync(u => u.Nickname == request.Username);
                if (usernameExists)
                    return BadRequest("Никнейм уже занят");
            }

            var newUser = new Models.User
            {
                Nickname = request.Username ?? string.Empty, // Если ник не передан, оставляем пустым
                CreatedAt = DateTime.UtcNow,
                IsAdmin = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Пользователь создан", UserId = newUser.Id });
        }

        // 5. Обновление никнейма пользователя (с проверкой на дублирование)
        [HttpPut("user/{id}/username")]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] UpdateUsernameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Никнейм не может быть пустым");

            bool usernameExists = await _context.Users.AnyAsync(u => u.Nickname == request.Username && u.Id != id);
            if (usernameExists)
                return BadRequest("Никнейм уже занят");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Пользователь не найден");

            user.Nickname = request.Username;
            await _context.SaveChangesAsync();

            return Ok("Никнейм обновлён");
        }
    }
    

}
