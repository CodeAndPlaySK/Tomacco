using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
        // Metodi specifici per Game
        Task<Game?> GetByCodeAsync(string gameCode);
        Task<Game?> GetByCodeWithDetailsAsync(string gameCode);
        Task<IEnumerable<Game>> GetGamesByStateAsync(GameState state);
        Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerTelegramId);
        Task<IEnumerable<Game>> GetActiveGamesAsync();
        Task<IEnumerable<Game>> GetRecentGamesAsync(int count = 10);
        Task<IEnumerable<Game>> GetGamesByCreatorAsync(string creatorTelegramId);
        Task<IEnumerable<Game>> GetJoinableGamesAsync();
        Task<bool> ExistsByCodeAsync(string gameCode);
        Task<int> GetPlayerGameCountAsync(string playerTelegramId, GameState? state = null);
        Task AddPlayerToGameAsync(string gameCode, string playerTelegramId);
        Task RemovePlayerFromGameAsync(string gameCode, string playerTelegramId);
    }

    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<Game?> GetByCodeAsync(string gameCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(g => g.Code == gameCode);
        }

        public async Task<Game?> GetByCodeWithDetailsAsync(string gameCode)
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .ThenInclude(c => c!.Slots)
                .ThenInclude(s => s.Building)
                .Include(g => g.Families)
                .ThenInclude(f => f.Members)
                .ThenInclude(m => m.Player)
                .Include(g => g.Families)
                .ThenInclude(f => f.Heroes)
                .Include(g => g.Turns)
                .FirstOrDefaultAsync(g => g.Code == gameCode);
        }

        public async Task<IEnumerable<Game>> GetGamesByStateAsync(GameState state)
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .Where(g => g.State == state)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerTelegramId)
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .Where(g => g.Players.Any(p => p.TelegramId == playerTelegramId))
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetActiveGamesAsync()
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .Where(g => g.State == GameState.NotStarted || g.State == GameState.Running)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetRecentGamesAsync(int count = 10)
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .OrderByDescending(g => g.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByCreatorAsync(string creatorTelegramId)
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .Where(g => g.PlayerCreatorId == creatorTelegramId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetJoinableGamesAsync()
        {
            return await _dbSet
                .Include(g => g.PlayerCreator)
                .Include(g => g.Players)
                .Include(g => g.City)
                .Where(g => g.State == GameState.NotStarted)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string gameCode)
        {
            return await _dbSet.AnyAsync(g => g.Code == gameCode);
        }

        public async Task<int> GetPlayerGameCountAsync(string playerTelegramId, GameState? state = null)
        {
            var query = _dbSet.Where(g => g.Players.Any(p => p.TelegramId == playerTelegramId));

            if (state.HasValue)
            {
                query = query.Where(g => g.State == state.Value);
            }

            return await query.CountAsync();
        }

        public async Task AddPlayerToGameAsync(string gameCode, string playerTelegramId)
        {
            var game = await _dbSet
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Code == gameCode);

            if (game == null)
                throw new InvalidOperationException($"Game with code '{gameCode}' not found");

            var player = await _context.Players.FindAsync(playerTelegramId);
            if (player == null)
                throw new InvalidOperationException($"Player with TelegramId '{playerTelegramId}' not found");

            if (!game.Players.Any(p => p.TelegramId == playerTelegramId))
            {
                game.Players.Add(player);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePlayerFromGameAsync(string gameCode, string playerTelegramId)
        {
            var game = await _dbSet
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Code == gameCode);

            if (game == null)
                throw new InvalidOperationException($"Game with code '{gameCode}' not found");

            var player = game.Players.FirstOrDefault(p => p.TelegramId == playerTelegramId);
            if (player != null)
            {
                game.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}
