namespace Domain.Entities
{
    public enum GameTurnState
    {
        Open, Resolving, Closed
    }
    public interface IGameTurn
    {
        IGame Game { get; set; }
        int Id { get; set; }
        GameTurnState State { get; set; }
    }
    public class GameTurn : IGameTurn
    {
        public IGame Game { get; set; }
        public int Id { get; set; }
        public GameTurnState State { get; set; }
    }
}
