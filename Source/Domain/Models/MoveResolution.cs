using Tomacco.Source.Entities;

namespace Tomacco.Source.Models
{
    public interface IMoveResolution
    {
        IHero Sender { get; set; }
        List<IHero> Receivers { get; set; }
        List<IMoveEffectResolution> Effects { get; set; }
    }

    public class MoveResolution : IMoveResolution
    {
        public IHero Sender { get; set; }
        public List<IHero> Receivers { get; set; }
        public List<IMoveEffectResolution> Effects { get; set; }
    }

    public interface IMoveEffectResolution;

    public interface IDamageMoveEffectResolution : IMoveEffectResolution
    {
        int Damage { get; set; }
    }

    public class DamageMoveEffectResolution : IDamageMoveEffectResolution
    {
        public int Damage { get; set; }
    }

    public interface IHealMoveEffectResolution : IMoveEffectResolution
    {
        public int Heal { get; set; }
        public HeroStatsEnumeration StatToHeal { get; set; }

    }

    public class HealMoveEffectResolution : IHealMoveEffectResolution
    {
        public int Heal { get; set; }
        public HeroStatsEnumeration StatToHeal { get; set; }
    }

    public interface IBuffMoveEffectResolution : IMoveEffectResolution
    {
        int BuffValue { get; set; }
        HeroStatsEnumeration StatBuffed { get; set; }
        int NumberRounds { get; set; }
    }

    public class BuffMoveEffectResolution : IBuffMoveEffectResolution
    {
        public int BuffValue { get; set; }
        public HeroStatsEnumeration StatBuffed { get; set; }
        public int NumberRounds { get; set; }
    }
}
