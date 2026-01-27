namespace Domain.Entities
{

    public enum GameState
    {
        NotStarted, Running, Ended
    }

    public interface IGame
    {
        string Code { get; set; }

        IPlayer PlayerCreator { get; set; }
        List<IPlayer> Players { get; set; }
        ICity City { get; set; }
        GameState State { get; set; }
        List<IGameTurn> Turns { get; set; }
    }

    
    public class Game : IGame
    {
        public string Code { get; set; }
        public IPlayer PlayerCreator { get; set; }
        public List<IPlayer> Players { get; set; }
        public ICity City { get; set; }
        public GameState State { get; set; }
        public List<IGameTurn> Turns { get; set; }
    }
}
