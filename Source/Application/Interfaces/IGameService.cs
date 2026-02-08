using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(string creatorTelegramId, List<string>? playerTelegramIds = null, int? citySlots = null);
        Task<Game?> GetGameByCodeAsync(string gameCode);
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<IEnumerable<Game>> GetGamesByStateAsync(GameState state);
        Task<IEnumerable<Game>> GetPlayerGamesAsync(string playerTelegramId);
        Task<int> GetPlayerGameCountAsync(string playerTelegramId, GameState? state = null);
        Task<Game> AddPlayerToGameAsync(string gameCode, string playerTelegramId);
        Task<bool> CanPlayerJoinGameAsync(string gameCode, string playerTelegramId);
        Task<Game> StartGameAsync(string gameCode, string requestingPlayerId);
        Task<Game> EndGameAsync(string gameCode, string requestingPlayerId);
    }
}
