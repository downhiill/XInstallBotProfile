using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;

namespace XInstallBotProfile.Controllers
{
    /*[Authorize(Roles = "Admin")]*/  // Это обеспечит доступ только для пользователей с ролью "Admin"
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. Получение списка пользователей
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // 2. Обновление флагов пользователя
        [HttpPut("user/{id}/flags")]
        public async Task<IActionResult> UpdateUserFlags(int id, [FromBody] UpdateFlagsRequest request)
        {
            request.Id = id; // Устанавливаем ID из маршрута
            var result = await _userService.UpdateUserFlags(request);
            return Ok(result);
        }

        // 4. Добавление пользователя
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUser(request);
            return Ok(result);
        }

        // 5. Обновление никнейма
        [HttpPut("user/{id}/username")]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] UpdateUsernameRequest request)
        {
            var result = await _userService.UpdateUsername(id, request);
            return Ok(result);
        }
        [HttpPatch("statistic/{id}")]
        public async Task<IActionResult> UpdateStatistic(UpdateStatisticRequest request)
        {
            var result = await _userService.UpdateStatistic(request);
            return Ok(result);
           
        }


        // 6. Общий API для сохранения всех изменений пользователя (ник и флаги)
        [HttpPut("user/save")]
        public async Task<IActionResult> SaveUserChanges([FromBody] SaveUserRequest request)
        {
            await _userService.SaveUserChanges(request);
            return Ok("Изменения сохранены");
        }

        [HttpGet("generateUserData")]
        public IActionResult GenerateUserData()
        {
            try
            {
                // Генерируем случайные данные для пользователя
                var userId = new Random().Next(1, 10000);  // Генерация случайного ID
                var userLogin = "user" + userId;  // Генерация логина
                var userPassword = "GeneratedPassword" + userId;  // Генерация пароля
                var userName = "User" + userId;  // Генерация имени

                // Возвращаем сгенерированные данные в ответе
                var userData = new
                {
                    Id = userId,
                    Login = userLogin,
                    Password = userPassword,
                    Name = userName
                };

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка на сервере.");
            }
        }

        [HttpPost("createUserRecord")]
        public async Task<IActionResult> CreateUserRecord([FromBody] CreateUserRecordRequest request)
        {
            await _userService.CreateUserRecord(request);
            return Ok("Запись создана");
        }

        [HttpPost("saveUserData")]
        public async Task<IActionResult> SaveUserData([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Некорректные данные пользователя.");

            try
            {
                int userId = await _userService.SaveUserAsync(user);
                return Ok(new { message = "Пользователь успешно сохранен!", userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при сохранении пользователя.");
            }
        }

        [HttpDelete("deleteUsers")]
        public async Task<IActionResult> DeleteUsers([FromBody] List<int> userIds)
        {
            try
            {
                await _userService.DeleteUser(userIds);
                return Ok(new { message = "Пользователи успешно удалены!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("deleteUserRecord")]
        public async Task<IActionResult> DeleteUserRecord([FromBody] long id)
        {
            try
            {
                await _userService.DeleteUserRecord(id);
                return Ok(new { message = "Пользователи успешно удалены!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
