using Domain.Entities;

namespace Domain.Repositories
{
    public interface IGameRepository
    {
        IGame? GetByCode(string codeGame);
        void InsertGame(IGame game);
        public List<IGame> GetAll();
    }

    public class GameRepositoryDummy : IGameRepository
    {
        private readonly Dictionary<string, IGame> _games = [];

        public IGame? GetByCode(string codeGame)
        {
            return _games.GetValueOrDefault(codeGame);
        }

        public void InsertGame(IGame game)
        {
            _games.Add(game.Code, game);
        }

        public List<IGame> GetAll()
        {
            return _games.Values.ToList();
        }
    }
}
