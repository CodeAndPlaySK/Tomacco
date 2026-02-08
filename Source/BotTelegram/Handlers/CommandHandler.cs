using Application.Interfaces;
using BotTelegram.Handlers.SubHandlers;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using BotTelegram.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BotTelegram.Handlers
{
    public class CommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IPlayerService _playerService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandRegistry _commandRegistry;

        public CommandHandler(
            ITelegramBotClient botClient,
            IPlayerService playerService,
            IAuditService auditService,
            ILogger<CommandHandler> logger,
            IServiceProvider serviceProvider,
            CommandRegistry commandRegistry)
        {
            _botClient = botClient;
            _playerService = playerService;
            _auditService = auditService;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _commandRegistry = commandRegistry;
        }

        public async Task HandleCommandAsync(
            long chatId,
            string telegramId,
            string username,
            string? userLanguageCode,
            string messageText,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command received from {Username} ({TelegramId}): {Command}",
                username, telegramId, messageText);

            // Recupera il player e determina la lingua
            var player = await _playerService.GetPlayerByTelegramIdAsync(telegramId);
            var languageCode = player?.LanguageCode ?? userLanguageCode ?? "en";

            // Audit log
            await _auditService.LogAsync(AuditAction.CommandExecuted,
                "Command",
                messageText.Split(' ')[0],
                telegramId,
                username,
                additionalInfo: messageText
            );

            // Crea il contesto
            var context = new CommandContext(
                chatId,
                telegramId,
                username,
                userLanguageCode,
                languageCode,
                messageText,
                player,
                cancellationToken
            );

            string response;

            // Gestione comandi speciali (callback)
            if (_commandRegistry.IsSpecialCommand(messageText))
            {
                var setLanguageHandler = _serviceProvider.GetRequiredService<SetLanguageCommandHandler>();
                response = await setLanguageHandler.HandleAsync(context);
            }
            else
            {
                // Trova e esegui il handler appropriato
                var handler = _commandRegistry.GetHandler(messageText, _serviceProvider);

                if (handler != null)
                {
                    response = await handler.HandleAsync(context);
                }
                else
                {
                    var localization = _serviceProvider.GetRequiredService<ILocalizationService>();
                    response = localization.GetString("command_unknown", languageCode);
                }
            }

            // Invia la risposta
            if (!string.IsNullOrEmpty(response))
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: response,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
