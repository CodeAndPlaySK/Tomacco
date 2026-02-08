using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Models
{
    public class ConsoleUser
    {
        public string TelegramId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "it";
        
        public bool IsLoggedIn => !string.IsNullOrEmpty(TelegramId);
    }
}
