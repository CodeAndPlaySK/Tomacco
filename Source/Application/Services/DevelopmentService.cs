using Application.Interfaces;
using Domain.Enums;
using Domain.Factories;
using Domain.Models;
using Domain.Services;

namespace Application.Services
{
    public class DevelopmentService : IDevelopmentService
    {

        public (Hero[], Move[]) GenerateTestMainEntities()
        {
            var moves = new Move[0];//_GenerateMoves();
            var heroClasses = _GenerateHeroClasses(moves);

            return (heroClasses, moves);
        }

        private Hero[] _GenerateHeroClasses(Move[] moves)
        {
            var warriorClass = new Hero()
            {
                Moves = [moves.First(m => m.Name == "Base attack"), moves.First(m => m.Name == "Super attack"), moves.First(m => m.Name == "Inspire force")]
            };
            var wizardClass = new Hero();
            var clericClass = new Hero();
            var scoundrelClass = new Hero();
            var guardianClass = new Hero();

            return [warriorClass, wizardClass, clericClass, scoundrelClass , guardianClass];
        }

        /*
        private Move[] _GenerateMoves()
        {
            var simpleAttackStrategy = new MoveTypeStrategy { Damage = () => new Random().Next(2) + 1,  };
            var powerAttackStrategy = new MoveTypeStrategy { Damage = () => new Random().Next(2) + 2 };
            var inspireForceHealStrategy = new MoveTypeStrategy { Value = () => 1, StatToHeal = HeroStatsEnumeration.MoralityPoints};
            var inspireForcePhyBuffStrategy = new MoveTypeStrategy { Value = () => 2, StatToBuff= HeroStatsEnumeration.Physic, NumberRounds = 1};
            var inspireForceMinBuffStrategy = new MoveTypeStrategy { Value = () => 1, StatToBuff = HeroStatsEnumeration.Mind, NumberRounds = 1 };

            var baseAttackWarrior = new Move { Name = "Base attack", Strategies = [simpleAttackStrategy], NumberTargets = 1, Range = 0, StatToHit = HeroStatsEnumeration.Physic , SimpleTargetMove = TargetType.Enemies};
            var powerfulAttackWarrior = new Move { Name = "Super attack", Strategies = [powerAttackStrategy], NumberTargets = 1, Range = 0, StatToHit = HeroStatsEnumeration.Physic , SimpleTargetMove = TargetType.Enemies };
            var inspireForceBuffWarrior = new Move { Name = "Inspire force", Strategies = [inspireForceHealStrategy, inspireForceMinBuffStrategy, inspireForcePhyBuffStrategy], SimpleTargetMove= TargetType.Allies };

            return [baseAttackWarrior, powerfulAttackWarrior, inspireForceBuffWarrior];
        }*/

        public Player[] CreateRandomPlayerForGame(int number)
        {
            var playerUsernames = new string[] { "Laendor", "Naman", "Sweettears", "Dauragon", "Vandar", "Vassilik", "Barbarancia"};
            var players = new List<Player>();

            for (int i = 0; i < number; i++)
            {
                var rand = new Random();
                var username = playerUsernames[rand.Next(playerUsernames.Length)];
                var player = new Player { TelegramId= $"{i:3}", Username = username};
                players.Add(player);
            }

            return players.ToArray();
        }

        public Family[] CreateRandomStartingFamilies(int familiesNumber)
        {
            var familyNames = new string[] { "Nat Star", "Russo", "Mariani", "Pizziol", "Monacelli", "Battistoni", "Crescenzi", "Codipietro", "Musk", "Stark", "Lannister"};
            var families = new List<Family>();

            for (int i = 0; i < familiesNumber; i++)
            {
                var rand = new Random();
                var familyName = familyNames[rand.Next(familyNames.Length)];
                var family = new Family { 
                    Heroes = [], Id = i, 
                    Name = familyName, 
                    Resources = new FamilyResources
                    {
                        Gold = 50, Influence = 0
                    } };
                families.Add(family);
            }

            return families.ToArray();
        }

        public FamilyOfPlayer[] CreateRandomStartingFamilyOfPlayers(int numberPlayers)
        {
            var players = CreateRandomPlayerForGame(numberPlayers);
            var families = CreateRandomStartingFamilies(numberPlayers);

            var familyOfPlayers = new List<FamilyOfPlayer>();

            for (int i = 0; i < numberPlayers; i++)
            {
                var familyOfPlayer = new FamilyOfPlayer { Family = families[i], Player = players[i] };
                familyOfPlayers.Add(familyOfPlayer);
            }

            return familyOfPlayers.ToArray();
        }

        public List<Hero> CreateRandomFirstLevelHeroes(Family[] families, (HeroClassType[] heroClasses, Move[] moves) mainEntities, int heroesNumber)
        {
            if (families.Length == 0) throw new ArgumentException(nameof(families));
            if (mainEntities.heroClasses.Length == 0) throw new ArgumentException(nameof(mainEntities.heroClasses));
            if (heroesNumber < 0) throw new ArgumentException(nameof(heroesNumber));

            var factory = new HeroFactory(new MoveStrategyFactory());

            var names = new string[] { "Simone", "Kristian", "Jennifer", "Francesco", "Umberto Mattia", "Roberto", "Federico", "Elisa", "Nicola", "Marco", "Sergio", "Andrea" };
            var heroes = new List<Hero>();

            for (int i = 0; i < heroesNumber; i++)
            {
                var rand = new Random();
                var name = names[rand.Next(names.Length)];
                var heroClass = mainEntities.heroClasses[rand.Next(mainEntities.heroClasses.Length)];
                var heroFamily = families[rand.Next(families.Length)];

                var hero = factory.CreateHeroFirstLevel(heroClass, i, name, heroFamily);
                heroes.Add(hero);
                heroFamily.Heroes.Add(hero);
            }

            return heroes;
        }

        public void PrintHeroesStatsOnConsole(List<Hero> heroes, bool isSortedByFamily = false)
        {
            var list = isSortedByFamily ? heroes.Select(h => h).OrderBy(h => h.Family.Name).ToList() : heroes;

            list.ForEach(hero =>
                Console.WriteLine(
                    $"ID: {hero.Id,2}] " +
                    $"{hero.Name,-15} " +
                    $"{hero.Family.Name,-10} " +
                    $"({hero.HeroClassType,-10}) " +
                    $"Lv:{hero.Stats.Level,2} " +
                    $"LP:{hero.Stats.LifePoints.Current,2}" +
                    $"/{hero.Stats.LifePoints.Max,2} " +
                    $"MP:{hero.Stats.MoralityPoints.Max,2}" +
                    $"/{hero.Stats.MoralityPoints.Max,2} " +
                    $"Phy:{hero.Stats.Physic,2} " +
                    $"Min:{hero.Stats.Mind,2} " +
                    $"Fai:{hero.Stats.Faith,2} " +
                    $"Spe:{hero.Stats.Speed,2} " +
                    $"Cha:{hero.Stats.Charisma,2} "+
                    $"# Moves: {hero.Moves.Count}  "
                )
            );
        }
    }
}
