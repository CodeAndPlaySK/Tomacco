using Domain.Entities;
using Domain.Factories;
using Domain.Repositories;
using Domain.Services;

namespace Application.Services
{
    public interface IGameService
    {
        IGame? GetGame(string codeGame);
        IGame CreateGame(IPlayer creatorOfGame, IPlayer[] otherPlayers);
        (bool isSuccess, string errorInCaseOfFailure) StartGame(IGame game);
        List<IGame> GetAllGames();
    }

    internal class GameService : IGameService
    {
        private readonly ICodeGameGenerator _codeGameGenerator;
        private readonly ICityNameGenerator _cityNameGenerator;
        private readonly IGameFactory _gameFactory;

        private readonly ICityService _cityService;
        private readonly IFamilyOfPlayerService _familyOfPlayerService;

        private readonly IGameRepository _gameRepository;

        public GameService(ICityService cityService, ICodeGameGenerator codeGameGenerator, IGameFactory gameFactory, ICityNameGenerator cityNameGenerator, IGameRepository gameRepository, IFamilyOfPlayerService familyOfPlayerService)
        {
            _cityService = cityService;
            _codeGameGenerator = codeGameGenerator;
            _gameFactory = gameFactory;
            _cityNameGenerator = cityNameGenerator;
            _gameRepository = gameRepository;
            _familyOfPlayerService = familyOfPlayerService;
        }

        public IGame? GetGame(string codeGame)
        {
            return _gameRepository.GetByCode(codeGame);
        }

        public IGame CreateGame(IPlayer creatorOfGame, IPlayer[] otherPlayers)
        {
            var city = _cityService.CreateCity(_cityNameGenerator.GenerateName());
            var familyOfPlayers = _familyOfPlayerService.CreateFamiliesFromPlayers([.. otherPlayers.ToList(), creatorOfGame]);
            city.FamilyOfPlayers.AddRange(familyOfPlayers.Select(x=>x).ToList());

            var codeGame = _codeGameGenerator.GenerateNewCode(codeGame => GetGame(codeGame) != null);
            var game = _gameFactory.Create(
                codeGame,
                city,
                creatorOfGame, 
                otherPlayers);
            _gameRepository.InsertGame(game);

            return game;
        }

        public (bool isSuccess, string errorInCaseOfFailure) StartGame(IGame game)
        {
            if (game.Players.Count == 0) return (false, "Cannot start a game without any player");
            if (game.State != GameState.NotStarted) return (false, $"The game is currently in the wrong state: '{game.State}'");
            
            game.State = GameState.Running; 
            return (isSuccess: true, errorInCaseOfFailure: string.Empty);
        }

        public List<IGame> GetAllGames()
        {
            return _gameRepository.GetAll();
        }
    }
}
