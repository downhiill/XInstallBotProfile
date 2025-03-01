using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Context;

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
            // Найдем пользователя по логину
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == model.Email);

            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            // Проверим пароль
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            // Если пароль верен, возвращаем уже сгенерированный ранее JWT токен
            return Ok(new { Token = user.JwtToken });
        }
    }
}
