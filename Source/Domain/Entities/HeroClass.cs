namespace Tomacco.Source.Entities
{
    public interface IHeroClass
    {
        string Name { get; set; }
        List<IMove> InitialMoves { get; set; }
    }

    public class HeroClass : IHeroClass
    {
        public string Name { get; set; }
        public List<IMove> InitialMoves { get; set; }
    }

    public interface IWarriorHeroClass : IHeroClass;
    public class WarriorHeroClass : HeroClass, IWarriorHeroClass
    {
        public new string Name => "Warrior";
    }

    public interface IWizardHeroClass : IHeroClass;
    public class WizardHeroClass : HeroClass, IWizardHeroClass
    {
        public new string Name => "Wizard";
    }

    public interface IClericHeroClass : IHeroClass;
    public class ClericHeroClass : HeroClass, IClericHeroClass
    {
        public new string Name => "Cleric";
    }

    public interface IGuardianHeroClass : IHeroClass;
    public class GuardianHeroClass : HeroClass, IGuardianHeroClass
    {
        public new string Name => "Guardian";
    }

    public interface IScoundrelHeroClass : IHeroClass;

    public class ScoundrelHeroClass : HeroClass, IScoundrelHeroClass
    {
        public new string Name => "Scoundrel";
    }
}
