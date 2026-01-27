using Domain.Entities;

namespace Domain.Factories
{
    public interface IGameFactory
    {
        public IGame Create(string code, ICity city, IPlayer creatorOfGame, IPlayer[] otherPlayers);

    }
    public class GameFactory : IGameFactory
    {
        public GameFactory()
        {
        }

        public IGame Create(string code, ICity city, IPlayer creatorOfGame, IPlayer[] otherPlayers)
        {
            return new Game
            {
                Code = code,
                City = city,
                State = GameState.NotStarted,
                Turns = [],
                PlayerCreator = creatorOfGame,
                Players = [..otherPlayers.ToList(), creatorOfGame]
            };
        }
    }
}
