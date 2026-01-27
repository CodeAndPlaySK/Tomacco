using Domain.Entities;

namespace Application.Services
{
    public interface IGameTurnService
    {
        IGameTurn CreateTurn(IGame game);

        bool StartTurn(IGameTurn gameTurn);

        bool ResolveTurn(IGameTurn gameTurn);

        IGameTurn GetGameTurn(int idGameTurn);
    }

    internal class GameTurnService : IGameTurnService
    {
        public IGameTurn CreateTurn(IGame game)
        {
            throw new NotImplementedException();
        }

        public bool StartTurn(IGameTurn gameTurn)
        {
            throw new NotImplementedException();
        }

        public bool ResolveTurn(IGameTurn gameTurn)
        {
            throw new NotImplementedException();
        }

        public IGameTurn GetGameTurn(int idGameTurn)
        {
            throw new NotImplementedException();
        }
    }
}
