namespace Domain.Entities
{
    public interface IPlayer
    {
        string TelegramId { get; set; }
        string Username { get; set; }

    }
    public class Player : IPlayer
    {
        public string TelegramId { get; set; }
        public string Username { get; set; }
    }
}
