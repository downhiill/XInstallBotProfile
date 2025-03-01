using XInstallBotProfile.Context;
using XInstallBotProfile.Generate;

namespace XInstallBotProfile.Service
{
    public class UserService
    {
        private ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(string login, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);  // Для безопасности храните хэш пароля
            var jwtToken = TokenGenerator.GenerateAccessToken(login);

            var user = new Models.User
            {
                Login = login,
                PasswordHash = passwordHash,
                JwtToken = jwtToken
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }

}
