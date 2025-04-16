using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;

namespace XInstallBotProfile.Controllers
{
     // Это обеспечит доступ только для пользователей с ролью "Admin"
    [ApiController]
    [Authorize(Roles = "Admin")]
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
            var result = await _userService.UpdateUserFlags(id, request);
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

        [HttpPatch("statistic-xinstall")]
        public async Task<IActionResult> UpdateStatisticXInstallApp(UpdateStatisticXInstallAppRequest request)
        {
            var result = await _userService.UpdateStatisticXInstallApp(request);
            return Ok(result);

        }
        [HttpPatch("statistic")]
        public async Task<IActionResult> UpdateStatistic(UpdateStatisticRequest request)
        {
            var result = await _userService.UpdateStatistic(request);
            return Ok(result);
           
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserById(id);
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
        public async Task<IActionResult> GenerateUserData()
        {
            var result = await _userService.GenerateUser();
            return Ok(result);
        }

        [HttpPost("createUserRecord")]
        public async Task<IActionResult> CreateUserRecord(int UserId, [FromBody] CreateUserRecordRequest request)
        {
            await _userService.CreateUserRecord(UserId, request);
            return Ok("Запись создана");
        }

        [HttpPost("createUserRecord-xinstallapp")]
        public async Task<IActionResult> CreateUserRecordXInstallApp(int UserId, [FromBody] CreateUserRecordXInstallAppRequest request)
        {
            await _userService.CreateUserRecordXInstallApp(UserId, request);
            return Ok("Запись создана");
        }

        [HttpPost("saveUserData")]
        public async Task<IActionResult> SaveUserData([FromBody] CreateUserRequest request)
        {
            if (request == null)
                return BadRequest("Некорректные данные пользователя.");

            try
            {
                _userService.SaveUserAsync(request);
                return Ok(new { message = "Пользователь успешно сохранен!" });
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
        public async Task<IActionResult> DeleteUserRecord([FromBody] List<long> ids)
        {
            try
            {
                await _userService.DeleteUserRecords(ids);
                return Ok(new { message = "Пользователи успешно удалены!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
