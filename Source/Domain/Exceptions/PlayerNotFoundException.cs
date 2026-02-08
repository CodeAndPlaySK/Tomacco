using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public string TelegramId { get; }

        public PlayerNotFoundException(string telegramId)
            : base($"Player with TelegramId '{telegramId}' not found")
        {
            TelegramId = telegramId;
        }
    }
}
