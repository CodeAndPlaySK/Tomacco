using Tomacco.Source.Entities;
using Tomacco.Source.Services;

namespace Tomacco
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IDevelopmentService devService = new DevelopmentService();
            ICity city = new City { FamilyOfPlayers = [], SlotCityBuildings = new ISlotCityBuilding[10] };
            IGame game = new Game { City = city, State = GameState.NotStarted };

            var familiesOfPlayers = devService.CreateRandomStartingFamilyOfPlayers(4);
            city.FamilyOfPlayers = familiesOfPlayers.Select(fp=>fp).ToList();

            var mainEntities = devService.GenerateTestMainEntities();

            var heroes = devService.CreateRandomFirstLevelHeroes(familiesOfPlayers.Select(fp=>fp.Family).ToArray(), mainEntities, 25);
            devService.PrintHeroesStatsOnConsole(heroes, isSortedByFamily: true);

            /*
            var warriors = heroes.Where(h => h.Class.GetType() == typeof(IWarriorHeroClass)).ToList();
            if (warriors.Count() < 2)
            {
                Console.WriteLine("Not enough warriors");
            }

            var attacker = warriors[0];
            var targets = warriors.Skip(1).ToList();
            Console.WriteLine($"Now {attacker.Name} attack the others: {targets.Select(w=>w.Name)}");

            while (targets.Any(t => t.Stats.LifePoints.Current > 0))
            {
                var move = attacker.Moves[new Random().Next(attacker.Moves.Count)];
                Console.WriteLine($"Move: {move.Name}");
                
            }
            */
        }
    }
}
