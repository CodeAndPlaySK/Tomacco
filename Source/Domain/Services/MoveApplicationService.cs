using Domain.Models;

namespace Domain.Services
{
    public interface IMoveApplicationService
    {
        void ApplyResolutions(IList<IMoveResolution> resolutions);
        MoveApplicationResult ApplyResolution(IMoveResolution resolution);
    }

    public class MoveApplicationService : IMoveApplicationService
    {
        public void ApplyResolutions(IList<IMoveResolution> resolutions)
        {
            foreach (var resolution in resolutions)
            {
                ApplyResolution(resolution);
            }
        }

        public MoveApplicationResult ApplyResolution(IMoveResolution resolution)
        {
            var result = new MoveApplicationResult
            {
                Sender = resolution.Sender,
                IsHit = resolution.IsHit
            };

            if (!resolution.IsHit)
            {
                result.Message = $"{resolution.Sender.Name}'s attack missed!";
                return result;
            }

            foreach (var effect in resolution.Effects)
            {
                foreach (var receiver in resolution.Receivers)
                {
                    ApplyEffect(effect, receiver, result);
                }
            }

            return result;
        }

        private void ApplyEffect(IMoveEffectResolution effect, Hero target, MoveApplicationResult result)
        {
            switch (effect)
            {
                case DamageMoveEffectResolution damageEffect:
                    ApplyDamage(damageEffect, target, result);
                    break;

                case HealMoveEffectResolution healEffect:
                    ApplyHeal(healEffect, target, result);
                    break;

                case BuffMoveEffectResolution buffEffect:
                    ApplyBuff(buffEffect, target, result);
                    break;
            }
        }

        private void ApplyDamage(DamageMoveEffectResolution effect, Hero target, MoveApplicationResult result)
        {
            var actualDamage = Math.Max(0, effect.Damage - target.Stats.Defence);
            var wasRemoved = target.Stats.LifePoints.Remove(actualDamage);

            result.TargetResults.Add(new TargetEffectResult
            {
                Target = target,
                EffectType = MoveStrategyType.Attack,
                Value = actualDamage,
                Message = $"{target.Name} took {actualDamage} damage! HP: {target.Stats.LifePoints.Current}/{target.Stats.LifePoints.Max}"
            });

            // Check se il target è morto
            if (target.Stats.LifePoints.Current <= 0)
            {
                result.TargetResults.Last().Message += $" {target.Name} has been defeated!";
            }
        }

        private void ApplyHeal(HealMoveEffectResolution effect, Hero target, MoveApplicationResult result)
        {
            // Determina quale stat curare
            var healed = effect.StatToHeal switch
            {
                HeroStatsEnumeration.LifePoints => target.Stats.LifePoints.Add(effect.HealAmount),
                HeroStatsEnumeration.MoralityPoints => target.Stats.MoralityPoints.Add(effect.HealAmount),
                _ => false
            };

            var currentValue = effect.StatToHeal switch
            {
                HeroStatsEnumeration.LifePoints => target.Stats.LifePoints.Current,
                HeroStatsEnumeration.MoralityPoints => target.Stats.MoralityPoints.Current,
                _ => 0
            };

            result.TargetResults.Add(new TargetEffectResult
            {
                Target = target,
                EffectType = MoveStrategyType.Heal,
                Value = effect.HealAmount,
                Message = $"{target.Name} was healed for {effect.HealAmount}! {effect.StatToHeal}: {currentValue}"
            });
        }

        private void ApplyBuff(BuffMoveEffectResolution effect, Hero target, MoveApplicationResult result)
        {
            // TODO: Implementa sistema di buff temporanei
            // Per ora logga solo l'intenzione
            result.TargetResults.Add(new TargetEffectResult
            {
                Target = target,
                EffectType = MoveStrategyType.Buff,
                Value = effect.BuffValue,
                Message = $"{target.Name}'s {effect.StatBuffed} increased by {effect.BuffValue} for {effect.NumberRounds} rounds!"
            });
        }
    }

    public class MoveApplicationResult
    {
        public Hero Sender { get; set; } = null!;
        public bool IsHit { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<TargetEffectResult> TargetResults { get; set; } = new();
    }

    public class TargetEffectResult
    {
        public Hero Target { get; set; } = null!;
        public MoveStrategyType EffectType { get; set; }
        public int Value { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
