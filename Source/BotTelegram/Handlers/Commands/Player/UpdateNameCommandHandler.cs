using Application.Interfaces;
using BotTelegram.Handlers;
using BotTelegram.Services;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Handlers.Commands.Player
{
    public class UpdateNameCommandHandler : ICommandHandler
    {
        private readonly IPlayerService _playerService;
        private readonly ILocalizationService _localization;
        private readonly ILogger<UpdateNameCommandHandler> _logger;

        public string CommandName => "/updatename";

        public UpdateNameCommandHandler(
            IPlayerService playerService,
            ILocalizationService localization,
            ILogger<UpdateNameCommandHandler> logger)
        {
            _playerService = playerService;
            _localization = localization;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CommandContext context)
        {
            try
            {
                var parts = context.MessageText.Split(' ', 2);
                if (parts.Length < 2)
                {
                    return _localization.GetString("updatename_usage", context.LanguageCode);
                }

                var newUsername = parts[1].Trim();

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    return _localization.GetString("updatename_empty", context.LanguageCode);
                }

                if (newUsername.Length > 50)
                {
                    return "❌ Username troppo lungo (max 50 caratteri)";
                }

                if (context.Player == null)
                {
                    return _localization.GetString("not_registered", context.LanguageCode);
                }

                var oldUsername = context.Player.Username;
                context.Player.Username = newUsername;
                await _playerService.UpdatePlayerAsync(context.Player);

                _logger.LogInformation("Username changed from {OldUsername} to {NewUsername} for {TelegramId}",
                    oldUsername, newUsername, context.TelegramId);

                return _localization.GetString("updatename_success", context.LanguageCode, oldUsername, newUsername);
            }
            catch (PlayerNotFoundException)
            {
                return _localization.GetString("not_registered", context.LanguageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating name for {TelegramId}", context.TelegramId);
                return _localization.GetString("error_generic", context.LanguageCode, ex.Message);
            }
        }
    }
}
