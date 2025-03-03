using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Requests.Abstractions;
using XInstallBotProfile.Context;
using XInstallBotProfile.Controllers;
using XInstallBotProfile.Service.AdminPanelService.Models.Request;
using XInstallBotProfile.Service.AdminPanelService.Models.Response;

namespace XInstallBotProfile.Service.AdminPanelService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                        Flag1 = u.IsDsp,
                        Flag2 = u.IsDspInApp,
                        Flag3 = u.IsDspBanner
                    })
                    .ToListAsync()
            };
        }


        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            // Проверяем, занят ли уже ник
            var existingUser = _dbContext.Users
                .Where(u => u.Nickname == request.Username)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("Никнейм уже занят");
            }

            // Создаем нового пользователя
            var user = new UserModel
            {
                Username = request.Username,
                CreatedAt = DateTime.UtcNow,
                Flag1 = true,
                Flag2 = false,
                Flag3 = false
            };

            // Добавляем пользователя в базу
            await _dbContext.SaveChangesAsync();

            // Возвращаем модель ответа
            return new CreateUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                IsDsp = user.Flag1,
                IsDspInApp = user.Flag2,
                IsDspBanner = user.Flag3

            };
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

        public async Task DeleteUser(int id)
        {
            // Находим пользователя по ID
            var user = await GetUserByIdAsync(id);
            // Удаляем пользователя
            _dbContext.Users.Remove(user);

            // Сохраняем изменения в БД
            await _dbContext.SaveChangesAsync();
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

    }
}
