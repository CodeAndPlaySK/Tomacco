using Domain.Enums;
using Domain.Models;

namespace Domain.Factories
{
    public interface IHeroFactory
    {
        Hero CreateHeroFirstLevel(HeroClassType heroClassType, int id, string name, Family family);
    }

    public class HeroFactory : IHeroFactory
    {
        private readonly Dictionary<HeroClassType, Func<HeroStats>> _statsGenerators;
        private readonly IMoveStrategyFactory _moveStrategyFactory;

        public HeroFactory(IMoveStrategyFactory moveStrategyFactory)
        {
            _moveStrategyFactory = moveStrategyFactory ?? throw new ArgumentNullException(nameof(moveStrategyFactory));
            _statsGenerators = new Dictionary<HeroClassType, Func<HeroStats>>
            {
                { HeroClassType.Warrior, _GenerateWarriorStats },
                { HeroClassType.Wizard, _GenerateWizardStats },
                { HeroClassType.Cleric, _GenerateClericStats },
                { HeroClassType.Scoundrel, _GenerateScoundrelStats },
                { HeroClassType.Guardian, _GenerateGuardianStats }
            };
        }
        
        public Hero CreateHeroFirstLevel(HeroClassType heroClassType, int id, string name, Family family)
        {
            if (!_statsGenerators.TryGetValue(heroClassType, out var statsGenerator))
            {
                throw new NotSupportedException($"Hero Class '{heroClassType}' is not supported");
            }

            return new Hero
            {
                Id = id,
                Name = name,
                Family = family,
                HeroClassType = heroClassType,
                Stats = statsGenerator(),
                Moves = _GetInitialMovesForClass(heroClassType),
                CreatedAt = DateTime.UtcNow
            };
        }
        
        #region Stats Generators

        private HeroStats _GenerateWarriorStats()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 8),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 10),
                Physic = rand.Next(3) + 3,
                Mind = rand.Next(2),
                Faith = rand.Next(2),
                Speed = rand.Next(3) + 1,
                Charisma = rand.Next(5) + 1,
            };
        }

        private HeroStats _GenerateWizardStats()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(4) + 4),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(4) + 10),
                Physic = rand.Next(2) + 1,
                Mind = rand.Next(3) + 3,
                Faith = rand.Next(2) + 1,
                Speed = rand.Next(4) + 1,
                Charisma = rand.Next(5) + 1,
            };
        }

        private HeroStats _GenerateClericStats()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 4),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 10),
                Physic = rand.Next(3) + 1,
                Mind = rand.Next(3) + 1,
                Faith = rand.Next(3) + 3,
                Speed = rand.Next(4) + 1,
                Charisma = rand.Next(5) + 1,
            };
        }

        private HeroStats _GenerateScoundrelStats()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 5),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 8),
                Physic = rand.Next(2) + 2,
                Mind = rand.Next(3) + 1,
                Faith = rand.Next(2),
                Speed = rand.Next(3) + 3,
                Charisma = rand.Next(4) + 2,
            };
        }

        private HeroStats _GenerateGuardianStats()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(4) + 10),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 8),
                Physic = rand.Next(3) + 2,
                Mind = rand.Next(2) + 1,
                Faith = rand.Next(3) + 2,
                Speed = rand.Next(2) + 1,
                Charisma = rand.Next(4) + 1,
            };
        }

        #endregion

        private List<Move> _GetInitialMovesForClass(HeroClassType heroClassTypeType)
        {
            return heroClassTypeType switch
            {
                HeroClassType.Warrior => new List<Move>
                {
                    new Move
                    {
                        Name = "Slash",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Physic,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("Slash damage", minDamage: 2, maxDamage: 5)]
                    },
                    new Move
                    {
                        Name = "Super slash",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Physic,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("Slash damage", minDamage: 3, maxDamage: 9)]
                    }
                },

                HeroClassType.Wizard => new List<Move>
                {
                    new Move { 
                        Name = "Fireball",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Mind,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("Fire Damage", minDamage: 2, maxDamage: 10)]
                    },
                    new Move
                    {
                        Name = "Magic Missile",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Mind,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("True Damage", minDamage: 3, maxDamage: 17)]
                    }
                },

                HeroClassType.Cleric => new List<Move>
                {
                    new Move
                    {
                        Name = "Heal",
                        MoveType = MoveType.Area,
                        Strategies = [
                            _moveStrategyFactory.CreateHealStrategy("Heal Life", statToHeal: HeroStatsEnumeration.LifePoints, minValue: 1, maxValue: 10)]
                    },
                    new Move
                    {
                        Name = "Smite",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Faith,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("True damage", fixedDamage: 6)]
                    }
                },

                HeroClassType.Scoundrel => new List<Move>
                {
                    new Move
                    {
                        Name = "Backstab",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Speed,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("True damage", minDamage: 3, maxDamage: 12)]
                    },
                    new Move
                    {
                        Name = "Poison Dart",
                        MoveType = MoveType.Simple,
                        StatToHit = HeroStatsEnumeration.Speed,
                        NumberTargets = 1,
                        Strategies = [_moveStrategyFactory.CreateAttackStrategy("Poison damage", minDamage: 2, maxDamage: 8)]
                    }
                },

                HeroClassType.Guardian => new List<Move>
                {
                    new Move
                    {
                        Name = "Taunt",
                        MoveType = MoveType.Simple,
                        Strategies = [_moveStrategyFactory.CreateBuffStrategy("Taunt debuff", HeroStatsEnumeration.Defence, buffValue: 2, numberOfRounds: 3)]
                    },
                    new Move {Name = "Counter"}
                },

                _ => new List<Move>()
            };
        }
    }
}
