using Domain.Models;

namespace Domain.Factories
{
    public interface IMoveStrategyFactory
    {
        MoveTypeStrategy CreateAttackStrategy(string name, int fixedDamage);
        MoveTypeStrategy CreateAttackStrategy(string name, int minDamage, int maxDamage);
        MoveTypeStrategy CreateBuffStrategy(string name, HeroStatsEnumeration statToBuff, int buffValue, int numberOfRounds);
        MoveTypeStrategy CreateBuffStrategy(string name, HeroStatsEnumeration statToBuff, int minValue, int maxValue, int numberOfRounds);
        MoveTypeStrategy CreateHealStrategy(string name, HeroStatsEnumeration statToHeal, int healValue);
        MoveTypeStrategy CreateHealStrategy(string name, HeroStatsEnumeration statToHeal, int minValue, int maxValue);

    }

    public class MoveStrategyFactory : IMoveStrategyFactory
    {
        public MoveTypeStrategy CreateAttackStrategy(string name, int fixedDamage)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Attack,
                FixedValue = fixedDamage
            };
        }

        public MoveTypeStrategy CreateAttackStrategy(string name, int minDamage, int maxDamage)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Attack,
                MinValue = minDamage,
                MaxValue = maxDamage
            };
        }

        public MoveTypeStrategy CreateBuffStrategy(string name, HeroStatsEnumeration statToBuff, int buffValue, int numberOfRounds)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Buff,
                FixedValue = buffValue,
                StatToBuff = statToBuff,
                NumberRounds = numberOfRounds
            };
        }

        public MoveTypeStrategy CreateBuffStrategy(string name, HeroStatsEnumeration statToBuff, int minValue, int maxValue, int numberOfRounds)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Buff,
                MinValue = minValue,
                MaxValue = maxValue,
                StatToBuff = statToBuff,
                NumberRounds = numberOfRounds
            };
        }

        public MoveTypeStrategy CreateHealStrategy(string name, HeroStatsEnumeration statToHeal, int healValue)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Heal,
                FixedValue = healValue,
                StatToHeal = statToHeal
            };
        }

        public MoveTypeStrategy CreateHealStrategy(string name, HeroStatsEnumeration statToHeal, int minValue, int maxValue)
        {
            return new MoveTypeStrategy
            {
                Name = name,
                StrategyType = MoveStrategyType.Heal,
                MinValue = minValue,
                MaxValue = maxValue,
                StatToHeal = statToHeal
            };
        }
    }
}
