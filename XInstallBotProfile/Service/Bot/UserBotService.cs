using XInstallBotProfile.Context;
using XInstallBotProfile.Generate;

namespace XInstallBotProfile.Service.Bot
{
    public class UserBotService
    {
        private ApplicationDbContext _context;

        public UserBotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(string login, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);  // Для безопасности храните хэш пароля

            // Получаем максимальный userId из базы данных и инкрементируем его для нового пользователя
            var userId = _context.Users.Max(u => (int?)u.Id) ?? 0 + 1;  // Если пользователей нет, начнем с 1

            var role = "User";  // Пример роли, можно сделать динамическим

            // Генерация JWT токена с userId и ролью
            var jwtToken = TokenGenerator.GenerateAccessToken(login, userId, role);

            var user = new Models.User
            {
                Login = login,
                PasswordHash = passwordHash,
                JwtToken = jwtToken,
                Role = role,
                IsDsp = true
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }


}
