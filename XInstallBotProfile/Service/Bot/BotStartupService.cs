using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace XInstallBotProfile.Service.Bot
{
    public class BotStartupService : BackgroundService
    {
        private readonly string _botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
        private readonly BotService _botService;
        private readonly ILogger<BotStartupService> _logger;

        public BotStartupService(BotService botService, ILogger<BotStartupService> logger)
        {
            _botService = botService;
            _logger = logger;
            _botToken = _botService.BotToken;  
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var botClient = new TelegramBotClient(_botToken);

            // Use array instead of list
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message } // Only receive messages
            };

            // Message handler
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken
            );

            // Wait for completion
            await Task.CompletedTask;
        }

        // Update handler
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await _botService.HandleUpdateAsync(update);
        }

        // Error handler
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error handling updates");
            return Task.CompletedTask;
        }
    }

}
