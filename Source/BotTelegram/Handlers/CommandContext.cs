using Domain.Models;

namespace BotTelegram.Handlers
{
    public class CommandContext
    {
        public long ChatId { get; set; }
        public string TelegramId { get; set; }
        public string Username { get; set; }
        public string? UserLanguageCode { get; set; }
        public string LanguageCode { get; set; }
        public string MessageText { get; set; }
        public Player? Player { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public CommandContext(
            long chatId,
            string telegramId,
            string username,
            string? userLanguageCode,
            string languageCode,
            string messageText,
            Player? player,
            CancellationToken cancellationToken)
        {
            ChatId = chatId;
            TelegramId = telegramId;
            Username = username;
            UserLanguageCode = userLanguageCode;
            LanguageCode = languageCode;
            MessageText = messageText;
            Player = player;
            CancellationToken = cancellationToken;
        }
    }
}
