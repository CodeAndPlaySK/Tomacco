using Application.Interfaces;
using BotTelegram.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using BotTelegram.Handlers;

namespace TelegramBot.Handlers.Commands.Systrem
{
    public class LanguageCommandHandler : ICommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILocalizationService _localization;
        private readonly ILogger<LanguageCommandHandler> _logger;

        public string CommandName => "/language";

        public LanguageCommandHandler(
            ITelegramBotClient botClient,
            ILocalizationService localization,
            ILogger<LanguageCommandHandler> logger)
        {
            _botClient = botClient;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🇮🇹 Italiano", "/setlang_it"),
                    InlineKeyboardButton.WithCallbackData("🇬🇧 English", "/setlang_en"),
                    InlineKeyboardButton.WithCallbackData("🇩🇪 Deutsch", "/setlang_de")
                }
            });

            await _botClient.SendMessage(
                chatId: context.ChatId,
                text: _localization.GetString("language_select", context.LanguageCode),
                parseMode: ParseMode.Html,
                replyMarkup: keyboard,
                cancellationToken: context.CancellationToken
            );

            return string.Empty; // Messaggio già inviato
        }
    }
}
