namespace Domain.Models
{
    public interface IMoveResolution
    {
        Hero Sender { get; }
        IList<Hero> Receivers { get; }
        IList<IMoveEffectResolution> Effects { get; }
        bool IsHit { get; }
    }

    public class MoveResolution : IMoveResolution
    {
        public Hero Sender { get; set; } = null!;
        public IList<Hero> Receivers { get; set; } = new List<Hero>();
        public IList<IMoveEffectResolution> Effects { get; set; } = new List<IMoveEffectResolution>();
        public bool IsHit { get; set; } = true;
    }

    // Interfaccia base per gli effetti
    public interface IMoveEffectResolution
    {
        MoveStrategyType EffectType { get; }
    }

    // Effetto danno
    public class DamageMoveEffectResolution : IMoveEffectResolution
    {
        public MoveStrategyType EffectType => MoveStrategyType.Attack;
        public int Damage { get; set; }
    }

    // Effetto buff
    public class BuffMoveEffectResolution : IMoveEffectResolution
    {
        public MoveStrategyType EffectType => MoveStrategyType.Buff;
        public int BuffValue { get; set; }
        public HeroStatsEnumeration StatBuffed { get; set; }
        public int NumberRounds { get; set; }
    }

    // Effetto heal
    public class HealMoveEffectResolution : IMoveEffectResolution
    {
        public MoveStrategyType EffectType => MoveStrategyType.Heal;
        public int HealAmount { get; set; }
        public HeroStatsEnumeration StatToHeal { get; set; }
    }

    // Risultato miss
    public class MissMoveResolution : IMoveResolution
    {
        public Hero Sender { get; set; } = null!;
        public IList<Hero> Receivers { get; set; } = new List<Hero>();
        public IList<IMoveEffectResolution> Effects { get; set; } = new List<IMoveEffectResolution>();
        public bool IsHit => false;
    }
}
