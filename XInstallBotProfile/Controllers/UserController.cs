using Microsoft.AspNetCore.Mvc;
using XInstallBotProfile.Generate;
using XInstallBotProfile.Service;

namespace XInstallBotProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser()
        {
            // Генерируем логин и пароль
            string login = CredentialGenerator.GenerateLogin();
            string password = CredentialGenerator.GeneratePassword();

            // Регистрация пользователя
            _userService.RegisterUser(login, password);

            return Ok(new { Login = login, Password = password });
        }
    }

}
