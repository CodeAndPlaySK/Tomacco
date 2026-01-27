namespace Tomacco.Source.Entities
{
    public interface IMoveTypeStrategy
    {
        string Name { get; init; }
    }

    public class MoveTypeStrategy : IMoveTypeStrategy
    {
        public string Name { get; init; }
    }

    public interface IAttackTypeStrategy : IMoveTypeStrategy
    {
        public Func<int> Damage { get; init; }
    }

    public class AttackTypeStrategy : MoveTypeStrategy, IAttackTypeStrategy
    {
        public Func<int> Damage { get; init; }
    }

    public interface IBuffTypeStrategy : IMoveTypeStrategy
    {
        Func<int> Value { get; init; }
        HeroStatsEnumeration StatToBuff { get; init; }
        int NumberRounds { get; init; }
    }
    public class BuffTypeStrategy : MoveTypeStrategy, IBuffTypeStrategy
    {
        public Func<int> Value { get; init; }
        public HeroStatsEnumeration StatToBuff { get; init; }
        public int NumberRounds { get; init; }
    }

    public interface IHealTypeStrategy : IMoveTypeStrategy
    {
        Func<int> Value { get; init; }
        HeroStatsEnumeration StatToHeal { get; init; }
    }
    public class HealTypeStrategy : MoveTypeStrategy, IHealTypeStrategy
    {
        public Func<int> Value { get; init; }
        public HeroStatsEnumeration StatToHeal { get; init; }
    }
}
