using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using XInstallBotProfile.Context;
using XInstallBotProfile.Controllers;
using XInstallBotProfile.Exepction;
using XInstallBotProfile.Generate;
using XInstallBotProfile.Models;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetAllUsersResponse> GetAllUsers()
        {
            

            return new GetAllUsersResponse
            {
                UserResponse = await _dbContext.Users
                    .Select(u => new UserModel
                    {
                        Id = u.Id,
                        Username = u.Nickname,
                        CreatedAt = u.CreatedAt,
                        Login = u.Login, // Логин
                        PasswordHash = u.PasswordHash, // Хэш пароля
                        IsDsp = u.IsDsp,
                        IsDspInApp = u.IsDspInApp,
                        IsDspBanner = u.IsDspBanner
                    })
                    .ToListAsync()


            };
        }

        public async Task<GenerateUserResponse> GenerateUser()
        {
                // Получаем максимальный ID из базы данных
                var maxUserId = await _dbContext.Users.MaxAsync(u => (int?)u.Id) ?? 0;

                // Генерируем новый ID как максимальный + 1
                var userId = maxUserId + 1;

                // Генерация логина, пароля и имени
                var userLogin = "user" + userId;
                var userPassword = "GeneratedPassword" + userId;
                var userName = "User" + userId;

                // Создаем объект пользователя
                var user = new XInstallBotProfile.Models.User
                {
                    Id = userId,
                    Login = userLogin,
                    PasswordHash = HashPassword(userPassword), // Хэшируем пароль
                    Nickname = userName
                };

                return new GenerateUserResponse
                {
                    Id = user.Id,
                    Login = user.Login,
                    Password = userPassword,
                    Nickname = user.Nickname
                };
            
        }


        private string HashPassword(string password)
        {
            // Предполагаем, что у вас есть метод для хэширования пароля
            // Пример: использование BCrypt для хэширования пароля
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<GetStatisticResponse> GetStatistic(GetStatisticRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId == 0)
            {
                throw new UnauthorizedAccessException("Необходима авторизация для выполнения запроса.");
            }

            if (currentUserRole != "Admin" && request.UserId != currentUserId)
            {
                // Если пользователь не администратор и пытается запросить чужую информацию
                throw new ForbiddenAccessException("У вас нет прав для запроса этой информации.");
            }

            var statisticsQuery = _dbContext.UserStatistics
                .Where(us => us.UserId == request.UserId)

                .Where(us => us.Date >= request.StartDate && us.Date <= request.EndDate);

            var statistics = await statisticsQuery.ToListAsync();

            var statisticTotal = new StatisticTotal
            {
                Total = statistics.Sum(us => us.Total),
                TotalAck = statistics.Sum(us => us.Ack),
                TotalWin = statistics.Sum(us => us.Win),
                TotalImpsCount = statistics.Sum(us => us.ImpsCount),
                TotalClicksCount = statistics.Sum(us => us.ClicksCount),
                TotalStartsCount = statistics.Sum(us => us.StartsCount),
                TotalCompletesCount = statistics.Sum(us => us.CompletesCount),
            };

            // Рассчитываем средние показатели
            var statisticAverages = new StatisticAverages
            {
                AverageTotal = statistics.Average(us => us.Total),
                AverageAck = statistics.Average(us => us.Ack),
                AverageWin = statistics.Average(us => us.Win),
                AverageImpsCount = statistics.Average(us => us.ImpsCount),
                AverageClicksCount = statistics.Average(us => us.ClicksCount),
                AverageStartsCount = statistics.Average(us => us.StartsCount),
                AverageCompletesCount = statistics.Average(us => us.CompletesCount),
            };

            return new GetStatisticResponse
            {
                UserStatistics = statistics,
                Total = statisticTotal,
                Averages = statisticAverages // Добавляем средние показатели в ответ
            };
        }


        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            // Проверяем, занят ли уже ник
            var existingUser = await _dbContext.Users
                .Where(u => u.Nickname == request.Username)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("Никнейм уже занят");
            }

            // Генерируем логин и пароль
            var login = request.Username;
            var password = "GeneratedPassword123";


            // Создаем пользователя
            var user = new XInstallBotProfile.Models.User
            {
                Nickname = request.Username,
                CreatedAt = DateTime.UtcNow,
                Login = login,
                Role = "User",
                IsDsp = true,
                IsDspInApp = false,
                IsDspBanner = false,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Возвращаем ответ
            return new CreateUserResponse
            {
                Id = user.Id,
                Username = user.Nickname,
                CreatedAt = user.CreatedAt,
                Login = user.Login,
                IsDsp = user.IsDsp,
                IsDspInApp = user.IsDspInApp,
                IsDspBanner = user.IsDspBanner
                
            };
        }

        public async Task<bool> CreateUserRecord (CreateUserRecordRequest request)
        {
            var recordUser = new UserStatistic
            {
                Date = DateTime.UtcNow,
                Total = request.Total,
                Ack = request.Ack,
                Win = request.Win,
                ImpsCount = request.ImpsCount,
                ClicksCount = request.ClicksCount,
                StartsCount = request.StartsCount,
                CompletesCount = request.CompletesCount
            };
            _dbContext.UserStatistics.Add(recordUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<UpdateUsernameResponse> UpdateUsername(int id, UpdateUsernameRequest request)
        {
            var user = await GetUserByIdAsync(id);

            // Проверяем, есть ли другой пользователь с таким ником
            bool nicknameTaken = await _dbContext.Users.AnyAsync(u => u.Nickname == request.Username && u.Id != id);
            if (nicknameTaken)
            {
                throw new Exception("Никнейм уже занят");
            }

            user.Nickname = request.Username;
            await _dbContext.SaveChangesAsync();

            return new UpdateUsernameResponse { Id = user.Id, Username = user.Nickname };
        }

        public async Task<UpdateFlagsResponse> UpdateUserFlags(UpdateFlagsRequest request)
        {
            var user = await GetUserByIdAsync(request.Id);

            user.IsDsp = request.Flag1;
            user.IsDspInApp = request.Flag2;
            user.IsDspBanner = request.Flag3;

            await _dbContext.SaveChangesAsync();

            return new UpdateFlagsResponse { Id = user.Id, Flag1 = user.IsDsp, Flag2 = user.IsDspInApp, Flag3 = user.IsDspBanner };
        }

        public async Task<bool> UpdateStatistic(UpdateStatisticRequest request)
        {
            // Находим статистику по ID
            var statistic = await _dbContext.UserStatistics
                .FirstOrDefaultAsync(us => us.Id == request.Id);

            if (statistic == null)
            {
                // Возвращаем false, если статистика не найдена
                return false;
            }

            // В зависимости от ключа, обновляем нужное поле
            switch (request.Key.ToLower())
            {
                case "total":
                    statistic.Total = request.Value;
                    break;
                case "ack":
                    statistic.Ack = request.Value;
                    break;
                case "win":
                    statistic.Win = request.Value;
                    break;
                case "impscount":
                    statistic.ImpsCount = request.Value;
                    break;
                case "showrate":
                    statistic.ShowRate = Convert.ToDecimal(request.Value);
                    break;
                case "clickscount":
                    statistic.ClicksCount = request.Value;
                    break;
                case "ctr":
                    statistic.Ctr = request.Value;
                    break;
                case "startscount":
                    statistic.StartsCount = request.Value;
                    break;
                case "completescount":
                    statistic.CompletesCount = request.Value;
                    break;
                case "vtr":
                    statistic.Vtr = request.Value;
                    break;
                default:
                    // Возвращаем false, если ключ некорректен
                    return false;
            }

            // Сохраняем изменения в базе данных
            await _dbContext.SaveChangesAsync();

            // Возвращаем true, если операция прошла успешно
            return true;
        }


        public async Task DeleteUser(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                throw new ArgumentException("Список ID пользователей не может быть пустым.");

            // Получаем всех пользователей с указанными ID
            var users = await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            if (users.Count == 0)
                throw new Exception("Пользователи с указанными ID не найдены.");

            // Удаляем пользователей
            _dbContext.Users.RemoveRange(users);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserRecord(long id)
        {
            // Находим запись по ID
            var recordUser = await _dbContext.UserStatistics
                .FirstOrDefaultAsync(us => us.Id == id);

            // Если запись не найдена, возвращаем false
            if (recordUser == null)
            {
                return false;
            }

            // Удаляем запись
            _dbContext.UserStatistics.Remove(recordUser);

            // Сохраняем изменения в базе данных
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task SaveUserChanges(SaveUserRequest request)
        {
            var user = await GetUserByIdAsync(request.Id);

            // Проверяем, есть ли изменения
            if (!HasUserChanged(user, request.UsernameRequest, request.FlagsRequest))
            {
                return; // Ничего не изменилось — сохранять не нужно
            }

            // Обновляем никнейм, если передан запрос на его изменение
            if (request.UsernameRequest != null)
            {
                bool nicknameTaken = await _dbContext.Users.AnyAsync(u => u.Nickname == request.UsernameRequest.Username && u.Id != request.Id);
                if (nicknameTaken)
                {
                    throw new Exception("Никнейм уже занят");
                }
                user.Nickname = request.UsernameRequest.Username;
            }

            // Обновляем флаги, если передан запрос на их изменение
            if (request.FlagsRequest != null)
            {
                user.IsDsp = request.FlagsRequest.Flag1;
                user.IsDspInApp = request.FlagsRequest.Flag2;
                user.IsDspBanner = request.FlagsRequest.Flag3;
            }

            // Сохраняем только если были изменения
            await _dbContext.SaveChangesAsync();
        }

        public void SaveUserAsync(CreateUserRequest request)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);  // Для безопасности храните хэш пароля

            // Получаем максимальный userId из базы данных и инкрементируем его для нового пользователя
            var userId = _dbContext.Users.Max(u => (int?)u.Id) ?? 0 + 1;  // Если пользователей нет, начнем с 1

            var role = "User";  // Пример роли, можно сделать динамическим

            // Генерация JWT токена с userId и ролью
            var jwtToken = TokenGenerator.GenerateAccessToken(request.Login, userId, role);

            var user = new XInstallBotProfile.Models.User
            {
                Login = request.Login,
                PasswordHash = passwordHash,
                Nickname = request.Username,
                JwtToken = jwtToken,
                IsDsp = true
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        private async Task<XInstallBotProfile.Models.User> GetUserByIdAsync(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }
            return user;
        }

        private bool HasUserChanged(XInstallBotProfile.Models.User user, UpdateUsernameRequest usernameRequest, UpdateFlagsRequest flagsRequest)
        {
            if (usernameRequest != null && user.Nickname != usernameRequest.Username)
            {
                return true;
            }
            if (flagsRequest != null &&
                (user.IsDsp != flagsRequest.Flag1 ||
                 user.IsDspInApp != flagsRequest.Flag2 ||
                 user.IsDspBanner != flagsRequest.Flag3))
            {
                return true;
            }
            return false;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

    }
}
