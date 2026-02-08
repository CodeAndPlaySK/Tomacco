using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PassiveActionService : IPassiveActionService
    {
        private readonly GameDbContext _context;

        public PassiveActionService(GameDbContext context)
        {
            _context = context;
        }

        public async Task ApplyPassiveEffectsForTurnAsync(string gameCode, int turnNumber)
        {
            // Trova tutti gli edifici completati nel gioco
            var buildings = await _context.Buildings
                .Include(b => b.Template)
                    .ThenInclude(t => t.ActionTemplates)
                        .ThenInclude(at => at.Effects)
                .Include(b => b.Template)
                    .ThenInclude(t => t.ActionTemplates)
                        .ThenInclude(at => at.Conditions)
                .Include(b => b.FamilyOwner)
                .Where(b => b.IsCompleted)
                .ToListAsync();

            foreach (var building in buildings)
            {
                var passiveActions = building.Template.ActionTemplates
                    .Where(at => at.ActionType == ActionType.Passive);

                foreach (var passiveAction in passiveActions)
                {
                    // Verifica le condizioni
                    if (await CheckConditionsAsync(passiveAction, building.FamilyOwner))
                    {
                        // Applica gli effetti
                        await ApplyEffectsAsync(passiveAction, building);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<bool> CheckConditionsAsync(BuildingActionTemplate action, Family family)
        {
            if (!action.Conditions.Any())
                return true;

            foreach (var condition in action.Conditions)
            {
                var conditionMet = condition.ConditionType switch
                {
                    ConditionType.None => true,

                    ConditionType.HasHeroOfClass => await _context.Heroes
                        .AnyAsync(h => h.FamilyId == family.Id && h.HeroClassType == condition.RequiredHeroClass),

                    ConditionType.HasBuilding => await _context.Buildings
                        .AnyAsync(b => b.FamilyOwnerId == family.Id &&
                                      b.Template.KindType == condition.RequiredBuildingKind &&
                                      b.IsCompleted),

                    ConditionType.HasMinGold => family.Resources.Gold >= (condition.RequiredAmount ?? 0),

                    ConditionType.HasMinInfluence => family.Resources.Influence >= (condition.RequiredAmount ?? 0),

                    _ => true
                };

                if (!conditionMet)
                    return false;
            }

            return true;
        }

        private async Task ApplyEffectsAsync(BuildingActionTemplate action, Building building)
        {
            foreach (var effect in action.Effects.OrderBy(e => e.Order))
            {
                var value = CalculateEffectValue(effect);
                var family = building.FamilyOwner;

                switch (effect.EffectType)
                {
                    case EffectType.GainGold:
                        family.Resources.Gold += value;
                        break;

                    case EffectType.GainInfluence:
                        family.Resources.Influence += value;
                        break;

                        // Aggiungi altri effetti...
                }
            }

            await Task.CompletedTask;
        }

        private int CalculateEffectValue(ActionEffect effect)
        {
            if (effect.FixedValue.HasValue)
                return effect.FixedValue.Value;

            if (effect is { MinValue: not null, MaxValue: not null })
            {
                var random = new Random();
                return random.Next(effect.MinValue.Value, effect.MaxValue.Value + 1);
            }

            return 0;
        }
    }
}
