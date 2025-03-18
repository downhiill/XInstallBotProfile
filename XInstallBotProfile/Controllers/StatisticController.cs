using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using XInstallBotProfile.Exepction;
using XInstallBotProfile.Service.AdminPanelService;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XInstallBotProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : Controller
    {
        private readonly IUserService _userService;

        public StatisticController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatistic([FromQuery] GetStatisticRequest request)
        {
            try
            {
                // Получаем статистику, если все проверки прошли успешно
                var result = await _userService.GetStatistic(request);
                return Ok(result); // Возвращаем статус 200 с результатом
            }
            catch (UnauthorizedAccessException ex)
            {
                // Если пользователь не авторизован, возвращаем 401 (Unauthorized)
                return Unauthorized(new { message = ex.Message });
            }
            catch (ForbiddenAccessException ex)
            {
                return Forbid(); // Возвращаем статус 403
            }
            catch (Exception ex)
            {
                // Общая ошибка сервера
                return StatusCode(500, new { message = "Произошла ошибка при обработке запроса." });
            }
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportStatisticInExcel([FromQuery] GetStatisticRequest request)
        {
            try
            {
                var result = await _userService.ExportStatisticInExcel(request);
                return Ok(result);
            }
            //catch(UnauthorizedAccessException ex) 
            //{
            //    return Unauthorized(new { message = ex.Message });
            //}
            //catch(ForbiddenAccessException ex)
            //{
            //    return Forbid();
            //}
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка при обработке запроса." });
            }
            

        }


    }
}
