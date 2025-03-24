using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using XInstallBotProfile.Context;
using XInstallBotProfile.Generate;

namespace XInstallBotProfile.Service.Bot
{
    public class BotService
    {
        private readonly string _botToken;
        private readonly ApplicationDbContext _context;

        public BotService(string botToken, ApplicationDbContext context)
        {
            _botToken = botToken;
            _context = context;
        }

        public string BotToken => _botToken;
        public async Task HandleUpdateAsync(Update update)
        {
            var botClient = new TelegramBotClient(_botToken);

            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                if (messageText.ToLower() == "/start")
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

                    if (user == null)
                    {
                        // Пользователь не найден, создаем нового
                        var login = CredentialGenerator.GenerateLogin();
                        var password = CredentialGenerator.GeneratePassword();
                        var passwordHash = password;

                        // Получаем максимальный userId из базы данных и инкрементируем его для нового пользователя
                        var userId = await _context.Users.MaxAsync(u => (int?)u.Id) ?? 0 + 1; // Если пользователей нет, начнем с 1

                        var role = "User";  // Пример роли, можно сделать динамическим

                        var jwtToken = TokenGenerator.GenerateAccessToken(login, userId, role);

                        var newUser = new Models.User
                        {
                            Login = login,
                            PasswordHash = passwordHash,
                            JwtToken = jwtToken,
                            ChatId = chatId
                        };

                        _context.Users.Add(newUser);
                        await _context.SaveChangesAsync();

                        // Отправка сообщения пользователю
                        string welcomeMessage = $"Добро пожаловать! Ваш логин: {login}, пароль: {password}.";
                        await botClient.SendTextMessageAsync(chatId, welcomeMessage);
                    }
                    else
                    {
                        // Если пользователь уже есть в базе данных
                        string message = "Вы уже зарегистрированы.";
                        await botClient.SendTextMessageAsync(chatId, message);
                    }
                }
            }
        }
    }

}
