using Application.Interfaces;
using Application.Services;
using BotTelegram.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotTelegram.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TelegramBotService> _logger;

        public TelegramBotService(
            ITelegramBotClient botClient,
            IServiceProvider serviceProvider,
            ILogger<TelegramBotService> logger)
        {
            _botClient = botClient;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = []
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );

            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation("✅ Bot @{BotUsername} è online!", me.Username);
            Console.WriteLine($"✅ Bot @{me.Username} è online!");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Message is { } message && message.Text is { } messageText)
                {
                    var chatId = message.Chat.Id;
                    var telegramId = message.From?.Id.ToString() ?? "";
                    var username = message.From?.Username ?? message.From?.FirstName ?? "Unknown";
                    var userLanguageCode = message.From?.LanguageCode;

                    _logger.LogInformation("📨 Messaggio da @{Username} ({TelegramId}, lang: {Language}): {Message}",
                        username, telegramId, userLanguageCode ?? "N/A", messageText);

                    using var scope = _serviceProvider.CreateScope();
                    var commandHandler = new CommandHandler(
                        botClient,
                        scope.ServiceProvider.GetRequiredService<IPlayerService>(),
                        scope.ServiceProvider.GetRequiredService<IAuditService>(),
                        scope.ServiceProvider.GetRequiredService<ILogger<CommandHandler>>(),
                        scope.ServiceProvider,
                        scope.ServiceProvider.GetRequiredService<CommandRegistry>()
                    );

                    await commandHandler.HandleCommandAsync(chatId, telegramId, username, userLanguageCode, messageText, cancellationToken);
                }
                else if (update.CallbackQuery is { } callbackQuery)
                {
                    var chatId = callbackQuery.Message!.Chat.Id;
                    var telegramId = callbackQuery.From.Id.ToString();
                    var username = callbackQuery.From.Username ?? callbackQuery.From.FirstName ?? "Unknown";
                    var userLanguageCode = callbackQuery.From.LanguageCode;
                    var data = callbackQuery.Data ?? "";

                    _logger.LogInformation("🔘 Callback da @{Username} ({TelegramId}): {Data}",
                        username, telegramId, data);

                    using var scope = _serviceProvider.CreateScope();
                    var commandHandler = new CommandHandler(
                        botClient,
                        scope.ServiceProvider.GetRequiredService<IPlayerService>(),
                        scope.ServiceProvider.GetRequiredService<IAuditService>(),
                        scope.ServiceProvider.GetRequiredService<ILogger<CommandHandler>>(),
                        scope.ServiceProvider,
                        scope.ServiceProvider.GetRequiredService<CommandRegistry>()
                    );

                    await commandHandler.HandleCommandAsync(chatId, telegramId, username, userLanguageCode, data, cancellationToken);
                    await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Errore durante la gestione dell'update");
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "❌ Errore di polling");
            return Task.CompletedTask;
        }
    }
}
