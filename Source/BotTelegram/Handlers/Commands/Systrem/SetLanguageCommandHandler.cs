using Application.Interfaces;
using BotTelegram.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;

namespace BotTelegram.Handlers.SubHandlers
{
    public class SetLanguageCommandHandler : ICommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IPlayerService _playerService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<SetLanguageCommandHandler> _logger;

        public string CommandName => "/setlang";

        public SetLanguageCommandHandler(
            ITelegramBotClient botClient,
            IPlayerService playerService,
            ILocalizationService localization,
            ILogger<SetLanguageCommandHandler> logger)
        {
            _botClient = botClient;
            _playerService = playerService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var newLanguage = context.MessageText.Replace("/setlang_", "");
                await _playerService.UpdatePlayerLanguageAsync(context.TelegramId, newLanguage);

                var message = newLanguage switch
                {
                    "it" => _localization.GetString("language_changed", "it"),
                    "en" => _localization.GetString("language_changed_en", "en"),
                    "de" => _localization.GetString("language_changed_de", "de"),
                    _ => _localization.GetString("language_changed_en", "en")
                };

                await _botClient.SendMessage(
                    chatId: context.ChatId,
                    text: message,
                    parseMode: ParseMode.Html,
                    cancellationToken: context.CancellationToken
                );

                return string.Empty; // Messaggio già inviato
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing language for {TelegramId}", context.TelegramId);
                return $"❌ Error: {ex.Message}";
            }
        }
    }
}
