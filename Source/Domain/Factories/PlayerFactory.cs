using Domain.Models;

namespace Domain.Factories
{
    public interface IPlayerFactory
    {
        Player Build(string username, string telegramId);
    }
    public class PlayerFactory : IPlayerFactory
    {
        public Player Build(string username, string telegramId)
        {
            return new Player
            {
                TelegramId = telegramId,
                Username = username,
            };
        }
    }
}
