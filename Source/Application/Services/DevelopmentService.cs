snamespace Application.Services
{
    public interface IDevelopmentService
    {
        (IHeroClass[], IMove[]) GenerateTestMainEntities();

        IFamilyOfPlayer[] CreateRandomStartingFamilyOfPlayers(int numberPlayers);
        IPlayer[] CreateRandomPlayerForGame(int number);
        IFamily[] CreateRandomStartingFamilies(int familiesNumber);
        List<IHero> CreateRandomFirstLevelHeroes(IFamily[] families, (IHeroClass[] heroClasses, IMove[] moves) mainEntities, int heroesNumber);
        void PrintHeroesStatsOnConsole(List<IHero> heroes, bool isSortedByFamily);
        public (IGameService gameService, IPlayerService playerService) CreateServicesForDevelopment();
    }

    public class DevelopmentService : IDevelopmentService
    {

        public (IHeroClass[], IMove[]) GenerateTestMainEntities()
        {
            var moves = _GenerateMoves();
            var heroClasses = _GenerateHeroClasses(moves);

            return (heroClasses, moves);
        }

        private IHeroClass[] _GenerateHeroClasses(IMove[] moves)
        {
            var warriorClass = new WarriorHeroClass()
            {
                InitialMoves = [moves.First(m => m.Name == "Base attack"), moves.First(m => m.Name == "Super attack"), moves.First(m => m.Name == "Inspire force")]
            };
            var wizardClass = new WizardHeroClass();
            var clericClass = new ClericHeroClass ();
            var scoundrelClass = new ScoundrelHeroClass ();
            var guardianClass = new GuardianHeroClass ();

            return [warriorClass, wizardClass, clericClass, scoundrelClass , guardianClass];
        }

        private IMove[] _GenerateMoves()
        {
            var simpleAttackStrategy = new AttackTypeStrategy { Damage = () => new Random().Next(2) + 1,  };
            var powerAttackStrategy = new AttackTypeStrategy {Damage = () => new Random().Next(2) + 2 };
            var inspireForceHealStrategy = new HealTypeStrategy { Value = () => 1, StatToHeal = HeroStatsEnumeration.MoralityPoints};
            var inspireForcePhyBuffStrategy = new BuffTypeStrategy { Value = () => 2, StatToBuff= HeroStatsEnumeration.Physic, NumberRounds = 1};
            var inspireForceMinBuffStrategy = new BuffTypeStrategy { Value = () => 1, StatToBuff = HeroStatsEnumeration.Mind, NumberRounds = 1 };

            var baseAttackWarrior = new SimpleMove { Name = "Base attack", Strategies = [simpleAttackStrategy], NumberTargets = 1, Range = 0, StatToHit = HeroStatsEnumeration.Physic , TargetMove = TargetSimpleMoveEnum.Enemies };
            var powerfulAttackWarrior = new SimpleMove { Name = "Super attack", Strategies = [powerAttackStrategy], NumberTargets = 1, Range = 0, StatToHit = HeroStatsEnumeration.Physic , TargetMove = TargetSimpleMoveEnum.Enemies};
            var inspireForceBuffWarrior = new AreaMove { Name = "Inspire force", Strategies = [inspireForceHealStrategy, inspireForceMinBuffStrategy, inspireForcePhyBuffStrategy], TargetMove = TargetAreaMoveEnum.Allies};

            return [baseAttackWarrior, powerfulAttackWarrior, inspireForceBuffWarrior];
        }

        public IPlayer[] CreateRandomPlayerForGame(int number)
        {
            var playerUsernames = new string[] { "Laendor", "Naman", "Sweettears", "Dauragon", "Vandar", "Vassilik", "Barbarancia"};
            var players = new List<IPlayer>();

            for (int i = 0; i < number; i++)
            {
                var rand = new Random();
                var username = playerUsernames[rand.Next(playerUsernames.Length)];
                var player = new Player { TelegramId= $"{i:3}", Username = username};
                players.Add(player);
            }

            return players.ToArray();
        }

        public IFamily[] CreateRandomStartingFamilies(int familiesNumber)
        {
            var familyNames = new string[] { "Nat Star", "Russo", "Mariani", "Pizziol", "Monacelli", "Battistoni", "Crescenzi", "Codipietro", "Musk", "Stark", "Lannister"};
            var families = new List<IFamily>();

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

        public IFamilyOfPlayer[] CreateRandomStartingFamilyOfPlayers(int numberPlayers)
        {
            var players = CreateRandomPlayerForGame(numberPlayers);
            var families = CreateRandomStartingFamilies(numberPlayers);

            var familyOfPlayers = new List<IFamilyOfPlayer>();

            for (int i = 0; i < numberPlayers; i++)
            {
                var familyOfPlayer = new FamilyOfPlayer { Family = families[i], Player = players[i] };
                familyOfPlayers.Add(familyOfPlayer);
            }

            return familyOfPlayers.ToArray();
        }

        public List<IHero> CreateRandomFirstLevelHeroes(IFamily[] families, (IHeroClass[] heroClasses, IMove[] moves) mainEntities, int heroesNumber)
        {
            if (families.Length == 0) throw new ArgumentException(nameof(families));
            if (mainEntities.heroClasses.Length == 0) throw new ArgumentException(nameof(mainEntities.heroClasses));
            if (heroesNumber < 0) throw new ArgumentException(nameof(heroesNumber));

            var factory = new HeroFactory(
                new WarriorHeroFactory(mainEntities.heroClasses.OfType<IWarriorHeroClass>().First()), 
                new WizardHeroFactory(mainEntities.heroClasses.OfType<IWizardHeroClass>().First()), 
                new ClericHeroFactory(mainEntities.heroClasses.OfType<IClericHeroClass>().First()),
                new ScoundrelHeroFactory(mainEntities.heroClasses.OfType<IScoundrelHeroClass>().First()),
                new GuardianHeroFactory(mainEntities.heroClasses.OfType<IGuardianHeroClass>().First())
                );

            var names = new string[] { "Simone", "Kristian", "Jennifer", "Francesco", "Umberto Mattia", "Roberto", "Federico", "Elisa", "Nicola", "Marco", "Sergio", "Andrea" };
            var heroes = new List<IHero>();

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

        public void PrintHeroesStatsOnConsole(List<IHero> heroes, bool isSortedByFamily = false)
        {
            var list = isSortedByFamily ? heroes.Select(h => h).OrderBy(h => h.Family.Name).ToList() : heroes;

            list.ForEach(hero =>
                Console.WriteLine(
                    $"ID: {hero.Id,2}] " +
                    $"{hero.Name,-15} " +
                    $"{hero.Family.Name,-10} " +
                    $"({hero.Class.Name,-10}) " +
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

        public (IGameService gameService, IPlayerService playerService) CreateServicesForDevelopment()
        {
            var cityRepo = new CityRepositoryDyummy();
            var gameRepo = new GameRepositoryDummy();
            var familyOfPlayersRepo = new FamilyOfPlayerRepositoryDummy();
            var playerRepo = new PlayerRepositoryDummy();

            var cityService = new CityService(new CityFactory(), cityRepo);
            var familyOfPlayerService = new FamilyOfPlayerService(familyOfPlayersRepo);
            var playerService = new PlayerService(playerRepo, new PlayerFactory());

            var gameService = new GameService(cityService, new CodeGameGenerator(), new GameFactory(), new CityNameGenerator(), gameRepo, familyOfPlayerService);

            return (gameService, playerService);
        }
    }
}
