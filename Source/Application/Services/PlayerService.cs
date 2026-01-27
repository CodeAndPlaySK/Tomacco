using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Factories;
using Domain.Repositories;

namespace Application.Services
{
    public interface IPlayerService
    {
        bool CreateNewPlayer(string username, string telegramId);
        IPlayer? GetPlayer(string telegramId);
        List<IPlayer> GetAllPlayers();
    }
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerFactory _playerFactory;

        public PlayerService(IPlayerRepository repository, IPlayerFactory playerFactory)
        {
            _playerRepository = repository;
            _playerFactory = playerFactory;
        }

        public bool CreateNewPlayer(string username, string telegramId)
        {
            var player = _playerFactory.Build(username, telegramId);
            return _playerRepository.InsertPlayer(player);
        }

        public IPlayer? GetPlayer(string telegramId)
        {
            return _playerRepository.GetByTelegramId(telegramId);
        }

        public List<IPlayer> GetAllPlayers()
        {
            return _playerRepository.GetAll();
        }
    }
}
