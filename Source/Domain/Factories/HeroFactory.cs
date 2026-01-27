using Tomacco.Source.Entities;
using Tomacco.Source.Models;

namespace Domain.Entities.Factories
{
    public interface IHeroFactory
    {
        IHero CreateHeroFirstLevel(IHeroClass heroClass, int id, string name, IFamily family);
    }

    public class HeroFactory : IHeroFactory
    {
        private readonly IWarriorHeroFactory _warriorHeroFactory;
        private readonly IWizardHeroFactory _wizardHeroFactory;
        private readonly IClericHeroFactory _clericHeroFactory;
        private readonly IScoundrelHeroFactory _scoundrelHeroFactory;
        private readonly IGuardianHeroFactory _guardianHeroFactory;

        public HeroFactory(IWarriorHeroFactory warriorHeroFactory, IWizardHeroFactory wizardHeroFactory, IClericHeroFactory clericHeroFactory, IScoundrelHeroFactory scoundrelHeroFactory, IGuardianHeroFactory guardianHeroFactory)
        {
            _warriorHeroFactory = warriorHeroFactory;
            _wizardHeroFactory = wizardHeroFactory;
            _clericHeroFactory = clericHeroFactory;
            _scoundrelHeroFactory = scoundrelHeroFactory;
            _guardianHeroFactory = guardianHeroFactory;
        }

        public IHero CreateHeroFirstLevel(IHeroClass heroClass, int id, string name, IFamily family)
        {
            if (heroClass.GetType().IsAssignableTo(typeof(IWarriorHeroClass)))
            {
                return _warriorHeroFactory.CreateHeroFirstLevel(id, name, family);
            }

            if (heroClass.GetType().IsAssignableTo(typeof(IWizardHeroClass)))
            {
                return _wizardHeroFactory.CreateHeroFirstLevel(id, name, family);
            }

            if (heroClass.GetType().IsAssignableTo(typeof(IClericHeroClass)))
            {
                return _clericHeroFactory.CreateHeroFirstLevel(id, name, family);
            }

            if (heroClass.GetType().IsAssignableTo(typeof(IScoundrelHeroClass)))
            {
                return _scoundrelHeroFactory.CreateHeroFirstLevel(id, name, family);
            }

            if (heroClass.GetType().IsAssignableTo(typeof(IGuardianHeroClass)))
            {
                return _guardianHeroFactory.CreateHeroFirstLevel(id, name, family);
            }

            throw new NotSupportedException($"Hero Class '{heroClass.Name}' unsupported");
        }
    }

    public interface ICommonHeroFactory
    {
        IHero CreateHeroFirstLevel(int id, string name, IFamily family);
    }

    public abstract class CommonHeroFactory : ICommonHeroFactory
    {
        protected readonly IHeroClass heroClass;
        public CommonHeroFactory(IHeroClass heroClass)
        {
            this.heroClass = heroClass;
        }

        public IHero CreateHeroFirstLevel(int id, string name, IFamily family) =>
            new Hero
            {
                Id = id,
                Name = name,
                Family = family,
                Class = heroClass,
                Stats = CreateHeroStatsForFirstLevel(),
                Moves = CreateHeroMovesForFirstLevel()
            };

        private List<IMove> CreateHeroMovesForFirstLevel() => heroClass.InitialMoves != null ? heroClass.InitialMoves.Select(m => m).ToList() : [];

        protected abstract IHeroStats CreateHeroStatsForFirstLevel();
    }

    public interface IWarriorHeroFactory : ICommonHeroFactory;

    public class WarriorHeroFactory : CommonHeroFactory, IWarriorHeroFactory
    {
        public WarriorHeroFactory(IWarriorHeroClass warriorHeroClass) : base(warriorHeroClass)
        {
        }

        protected override IHeroStats CreateHeroStatsForFirstLevel()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(3)+8),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(3) + 10),
                Physic = rand.Next(3) + 3,
                Mind = rand.Next(2),
                Faith = rand.Next(2),
                Speed = rand.Next(3) + 1,
                Charisma = rand.Next(5) + 1,
            };
        }
    }

    public interface IWizardHeroFactory : ICommonHeroFactory;

    public class WizardHeroFactory : CommonHeroFactory, IWizardHeroFactory
    {

        public WizardHeroFactory(IWizardHeroClass wizardHeroClass) : base(wizardHeroClass)
        {
        }


        protected override IHeroStats CreateHeroStatsForFirstLevel()
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
    }

    public interface IClericHeroFactory : ICommonHeroFactory;

    public class ClericHeroFactory : CommonHeroFactory, IClericHeroFactory
    {
        public ClericHeroFactory(IClericHeroClass clericHeroClass) : base(clericHeroClass)
        {
        }

        protected override IHeroStats CreateHeroStatsForFirstLevel()
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
    }

    public interface IScoundrelHeroFactory : ICommonHeroFactory;

    public class ScoundrelHeroFactory : CommonHeroFactory, IScoundrelHeroFactory
    {
        public ScoundrelHeroFactory(IScoundrelHeroClass heroClass) : base(heroClass)
        {
        }

        protected override IHeroStats CreateHeroStatsForFirstLevel()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(1)),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(1)),
                Physic = rand.Next(1),
                Mind = rand.Next(1) ,
                Faith = rand.Next(1) ,
                Speed = rand.Next(1) ,
                Charisma = rand.Next(1),
            };
        }
    }

    public interface IGuardianHeroFactory : ICommonHeroFactory;

    public class GuardianHeroFactory : CommonHeroFactory, IGuardianHeroFactory
    {
        public GuardianHeroFactory(IGuardianHeroClass heroClass) : base(heroClass)
        {
        }

        protected override IHeroStats CreateHeroStatsForFirstLevel()
        {
            var rand = new Random();
            return new HeroStats
            {
                Level = 1,
                LifePoints = BarPoints.CreateFullBarPoints(rand.Next(1)),
                MoralityPoints = BarPoints.CreateFullBarPoints(rand.Next(1)),
                Physic = rand.Next(1),
                Mind = rand.Next(1),
                Faith = rand.Next(1),
                Speed = rand.Next(1),
                Charisma = rand.Next(1),
            };
        }
    }
}
