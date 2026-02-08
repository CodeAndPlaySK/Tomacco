using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    
    public class Player
    {
        [Key]
        public string TelegramId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string LanguageCode { get; set; }
    }
}
