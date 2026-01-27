using System.Diagnostics.CodeAnalysis;
using Tomacco.Source.Entities;
using Tomacco.Source.Models;

namespace Application.Services
{
    public interface IMoveResolveService;

    public class MoveResolverService : IMoveResolveService
    {
        public MoveResolverService() { }

        public List<IMoveResolution> UseMove(IHero user, IList<IHero> targets, IMove move)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            if (move == null) throw new ArgumentNullException(nameof(move));

            var result = new List<IMoveResolution>();

            if (move is ISimpleMove simpleMove)
            {
                if (targets.Count > simpleMove.NumberTargets) throw new ArgumentException("Too much targets");
                
                foreach (var target in targets)
                {
                    if (_IsAttackHit(user.Stats, target.Stats, simpleMove.StatToHit))
                    {
                        foreach (var moveStrategy in simpleMove.Strategies)
                        {
                            if (moveStrategy is IAttackTypeStrategy attackStrategy)
                            {
                                result.Add(new MoveResolution { Effects = [new DamageMoveEffectResolution { Damage = attackStrategy.Damage() }], Receivers = [target], Sender = user });
                                continue;
                            }

                            if (moveStrategy is IBuffTypeStrategy buffStrategy)
                            {
                                result.Add(new MoveResolution{Effects = [new BuffMoveEffectResolution{BuffValue = buffStrategy.Value(), StatBuffed = buffStrategy.StatToBuff, NumberRounds = buffStrategy.NumberRounds}] });
                            }

                            if (moveStrategy is IHealTypeStrategy healTypeStrategy)
                            {
                                result.Add(new MoveResolution { Effects = [new HealMoveEffectResolution{Heal = healTypeStrategy.Value(), StatToHeal = healTypeStrategy.StatToHeal}] });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool _IsAttackHit(IHeroStats attackStatsHero, IHeroStats defenceStatsHero, HeroStatsEnumeration? statToHit)
        {
            if (attackStatsHero == null) throw new ArgumentNullException(nameof(attackStatsHero));
            if (defenceStatsHero == null) throw new ArgumentNullException(nameof(defenceStatsHero));

            if (statToHit == null) return true;

            var probabilityToHit = _GetProbabilityToHit(attackStatsHero, defenceStatsHero, statToHit);
            var rdmValue = new Random().Next(100) + 1;
            return rdmValue < probabilityToHit;
        }

        private int _GetProbabilityToHit(IHeroStats attackStatsHero, IHeroStats defenceStatsHero, [DisallowNull] HeroStatsEnumeration? mainStat)
        {
            var attackAmount = _GetAttackAmount(attackStatsHero, mainStat);
            var defenceAmount = _GetDefenceAmount(defenceStatsHero);
            var ratioHit = attackAmount / (attackAmount + defenceAmount) * 100;
            return ratioHit;
        }

        private int _GetDefenceAmount(IHeroStats defenceStatsHero) => defenceStatsHero.Defence;

        private int _GetAttackAmount(IHeroStats attackStatsHero, HeroStatsEnumeration? mainStat) =>
            mainStat switch
            {
                HeroStatsEnumeration.Physic => attackStatsHero.Physic,
                HeroStatsEnumeration.Mind => attackStatsHero.Mind,
                HeroStatsEnumeration.Faith => attackStatsHero.Faith,
                _ => throw new ArgumentException("Stat not supported for hit move")
            };
    }
}
