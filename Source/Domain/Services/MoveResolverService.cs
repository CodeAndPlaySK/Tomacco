using System.Diagnostics.CodeAnalysis;
using Domain.Models;

namespace Domain.Services
{
    public interface IMoveResolverService
    {
        List<IMoveResolution> UseMove(Hero user, IList<Hero> targets, Move move);
        MoveResolution UseMoveOnSingleTarget(Hero user, Hero target, Move move);
    }

    public class MoveResolverService : IMoveResolverService
    {
        private readonly Random _random = new();

        public List<IMoveResolution> UseMove(Hero user, IList<Hero> targets, Move move)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(targets);
            ArgumentNullException.ThrowIfNull(move);

            return move.MoveType switch
            {
                MoveType.Simple => ResolveSimpleMove(user, targets, move),
                MoveType.Area => ResolveAreaMove(user, targets, move),
                MoveType.Self => ResolveSelfMove(user, move),
                _ => throw new ArgumentException($"Move type '{move.MoveType}' not supported")
            };
        }

        public MoveResolution UseMoveOnSingleTarget(Hero user, Hero target, Move move)
        {
            var results = UseMove(user, new List<Hero> { target }, move);
            return results.FirstOrDefault() as MoveResolution ?? new MoveResolution
            {
                Sender = user,
                Receivers = new List<Hero> { target },
                IsHit = false
            };
        }

        #region Move Type Resolvers

        private List<IMoveResolution> ResolveSimpleMove(Hero user, IList<Hero> targets, Move move)
        {
            var maxTargets = move.NumberTargets ?? 1;

            if (targets.Count > maxTargets)
            {
                throw new ArgumentException(
                    $"Too many targets. Max: {maxTargets}, Provided: {targets.Count}",
                    nameof(targets));
            }

            var results = new List<IMoveResolution>();

            foreach (var target in targets)
            {
                var isHit = IsAttackHit(user.Stats, target.Stats, move.StatToHit);

                if (isHit)
                {
                    var resolution = CreateResolutionForTarget(user, target, move);
                    results.Add(resolution);
                }
                else
                {
                    results.Add(new MissMoveResolution
                    {
                        Sender = user,
                        Receivers = new List<Hero> { target }
                    });
                }
            }

            return results;
        }

        private List<IMoveResolution> ResolveAreaMove(Hero user, IList<Hero> targets, Move move)
        {
            var results = new List<IMoveResolution>();

            // Area move colpisce tutti, niente check hit per target
            var resolution = new MoveResolution
            {
                Sender = user,
                Receivers = targets.ToList(),
                IsHit = true,
                Effects = new List<IMoveEffectResolution>()
            };

            foreach (var strategy in move.Strategies.OrderBy(s => s.Order))
            {
                var effect = CreateEffectFromStrategy(strategy);
                if (effect != null)
                {
                    resolution.Effects.Add(effect);
                }
            }

            results.Add(resolution);
            return results;
        }

        private List<IMoveResolution> ResolveSelfMove(Hero user, Move move)
        {
            var resolution = CreateResolutionForTarget(user, user, move);
            return new List<IMoveResolution> { resolution };
        }

        #endregion

        #region Resolution Creation

        private MoveResolution CreateResolutionForTarget(Hero user, Hero target, Move move)
        {
            var resolution = new MoveResolution
            {
                Sender = user,
                Receivers = new List<Hero> { target },
                IsHit = true,
                Effects = new List<IMoveEffectResolution>()
            };

            foreach (var strategy in move.Strategies.OrderBy(s => s.Order))
            {
                var effect = CreateEffectFromStrategy(strategy);
                if (effect != null)
                {
                    resolution.Effects.Add(effect);
                }
            }

            return resolution;
        }

        private IMoveEffectResolution? CreateEffectFromStrategy(MoveTypeStrategy strategy)
        {
            return strategy.StrategyType switch
            {
                MoveStrategyType.Attack => new DamageMoveEffectResolution
                {
                    Damage = strategy.GetValue()
                },

                MoveStrategyType.Buff => new BuffMoveEffectResolution
                {
                    BuffValue = strategy.GetValue(),
                    StatBuffed = strategy.StatToBuff
                        ?? throw new InvalidOperationException("Buff strategy must have StatToBuff"),
                    NumberRounds = strategy.NumberRounds
                        ?? throw new InvalidOperationException("Buff strategy must have NumberRounds")
                },

                MoveStrategyType.Heal => new HealMoveEffectResolution
                {
                    HealAmount = strategy.GetValue(),
                    StatToHeal = strategy.StatToHeal
                        ?? throw new InvalidOperationException("Heal strategy must have StatToHeal")
                },

                _ => null
            };
        }

        #endregion

        #region Hit Calculation

        private bool IsAttackHit(HeroStats attackerStats, HeroStats defenderStats, HeroStatsEnumeration? statToHit)
        {
            ArgumentNullException.ThrowIfNull(attackerStats);
            ArgumentNullException.ThrowIfNull(defenderStats);

            // Se non c'è stat to hit, colpisce sempre
            if (statToHit == null)
                return true;

            var hitProbability = CalculateHitProbability(attackerStats, defenderStats, statToHit.Value);
            var roll = _random.Next(1, 101); // 1-100

            return roll <= hitProbability;
        }

        private int CalculateHitProbability(HeroStats attackerStats, HeroStats defenderStats, HeroStatsEnumeration mainStat)
        {
            var attackValue = GetStatValue(attackerStats, mainStat);
            var defenceValue = defenderStats.Defence;

            // Evita divisione per zero
            var total = attackValue + defenceValue;
            if (total <= 0)
                return 50; // Default 50% se entrambi sono 0

            // Calcola con decimali poi converti
            var probability = (double)attackValue / total * 100;

            // Clamp tra 5% e 95%
            return Math.Clamp((int)probability, 5, 95);
        }

        private int GetStatValue(HeroStats stats, HeroStatsEnumeration stat)
        {
            return stat switch
            {
                HeroStatsEnumeration.Physic => stats.Physic,
                HeroStatsEnumeration.Mind => stats.Mind,
                HeroStatsEnumeration.Faith => stats.Faith,
                HeroStatsEnumeration.Speed => stats.Speed,
                HeroStatsEnumeration.Charisma => stats.Charisma,
                _ => throw new ArgumentException($"Stat '{stat}' not supported for hit calculation", nameof(stat))
            };
        }

        #endregion
    }
}
