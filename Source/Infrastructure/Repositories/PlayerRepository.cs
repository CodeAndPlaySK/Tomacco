using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetByTelegramIdAsync(string telegramId);
        Task<Player?> GetByUsernameAsync(string username);
        Task<IEnumerable<Player>> GetActivePlayersAsync();
        Task<IEnumerable<Player>> GetPlayersByLanguageAsync(string languageCode);
        Task<IEnumerable<Player>> SearchPlayersByUsernameAsync(string searchTerm);
        Task<bool> ExistsByTelegramIdAsync(string telegramId);
        Task UpdateLastLoginAsync(string telegramId);
    }

    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<Player?> GetByTelegramIdAsync(string telegramId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.TelegramId == telegramId);
        }

        public async Task<Player?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Username == username);
        }

        public async Task<IEnumerable<Player>> GetActivePlayersAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.LastLoginAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetPlayersByLanguageAsync(string languageCode)
        {
            return await _dbSet
                .Where(p => p.LanguageCode == languageCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> SearchPlayersByUsernameAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => p.Username.Contains(searchTerm))
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTelegramIdAsync(string telegramId)
        {
            return await _dbSet.AnyAsync(p => p.TelegramId == telegramId);
        }

        public async Task UpdateLastLoginAsync(string telegramId)
        {
            var player = await GetByTelegramIdAsync(telegramId);
            if (player != null)
            {
                player.LastLoginAt = DateTime.UtcNow;
                await UpdateAsync(player);
            }
        }
    }
}
