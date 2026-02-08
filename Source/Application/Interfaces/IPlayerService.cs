using Domain.Models;

namespace Application.Interfaces
{
    public interface IPlayerService
    {
        Task<Player> CreatePlayerAsync(string telegramId, string username, string languageCode = "en");
        Task<Player?> GetPlayerByTelegramIdAsync(string telegramId);
        Task<Player?> GetPlayerByUsernameAsync(string username);
        Task<bool> PlayerExistsAsync(string telegramId);
        Task<Player> UpdatePlayerAsync(Player player);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<Player> UpdatePlayerLanguageAsync(string telegramId, string languageCode);
    }
}
