using Domain.Entities;

namespace Domain.Factories
{
    public interface IPlayerFactory
    {
        IPlayer Build(string username, string telegramId);
    }
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer Build(string username, string telegramId)
        {
            return new Player
            {
                TelegramId = telegramId,
                Username = username,
            };
        }
    }
}
