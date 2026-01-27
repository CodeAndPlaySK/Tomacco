using System.Net.Http.Headers;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPlayerRepository
    {
        public IPlayer? GetByTelegramId(string telegramId);

        public bool InsertPlayer(IPlayer player);

        public bool UpdatePlayer(IPlayer player);

        public List<IPlayer> GetAll();
    }

    public class PlayerRepositoryDummy : IPlayerRepository
    {
        private readonly Dictionary<string, IPlayer> _players = [];

        public IPlayer? GetByTelegramId(string telegramId)
        {
            _players.TryGetValue(telegramId, out var player);
            return player;
        }

        public bool InsertPlayer(IPlayer player)
        {
            return _players.TryAdd(player.TelegramId, player);
        }

        public bool UpdatePlayer(IPlayer player)
        {
            if (!_players.ContainsKey(player.TelegramId)) return false;
            _players[player.TelegramId] = player;
            return true;
        }

        public List<IPlayer> GetAll()
        {
            return _players.Values.ToList();
        }
    }
}
