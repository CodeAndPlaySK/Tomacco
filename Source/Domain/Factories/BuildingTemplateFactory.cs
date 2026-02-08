using Domain.Enums;
using Domain.Models;

namespace Domain.Factories
{
    public class BuildingTemplateFactory
    {
        public BuildingTemplate CreateMine()
        {
            return new BuildingTemplate
            {
                Name = "Mine",
                KindType = BuildingKindType.Mine,
                CostructionCost = 50,
                TurnsToComplete = 2,
                ActionTemplates = new List<BuildingActionTemplate>
                {
                    // Azione passiva: guadagna 5 oro ogni turno
                    new BuildingActionTemplate
                    {
                        Name = "Gold Production",
                        Description = "Produces gold each turn",
                        ActionType = ActionType.Passive,
                        Effects = new List<ActionEffect>
                        {
                            new ActionEffect
                            {
                                EffectType = EffectType.GainGold,
                                TargetType = EffectTargetType.Self,
                                FixedValue = 5,
                                Order = 1
                            }
                        }
                    },
                    // Azione attiva: estrai cristalli (10 oro)
                    new BuildingActionTemplate
                    {
                        Name = "Extract Crystals",
                        Description = "Assign a hero to extract valuable crystals",
                        ActionType = ActionType.Active,
                        MaxHeroSlots = 1,
                        Effects = new List<ActionEffect>
                        {
                            new ActionEffect
                            {
                                EffectType = EffectType.GainGold,
                                TargetType = EffectTargetType.Self,
                                FixedValue = 10,
                                Order = 1
                            }
                        }
                    }
                }
            };
        }

        public BuildingTemplate CreateChurch()
        {
            return new BuildingTemplate
            {
                Name = "Church",
                KindType = BuildingKindType.Church,
                CostructionCost = 80,
                TurnsToComplete = 3,
                ActionTemplates = new List<BuildingActionTemplate>
                {
                    // Azione passiva con condizione
                    new BuildingActionTemplate
                    {
                        Name = "Divine Blessing",
                        Description = "Gain influence if you have a Cleric hero",
                        ActionType = ActionType.Passive,
                        Conditions = new List<ActionCondition>
                        {
                            new ActionCondition
                            {
                                ConditionType = ConditionType.HasHeroOfClass,
                                RequiredHeroClass = HeroClassType.Cleric,
                                RequiredAmount = 1
                            }
                        },
                        Effects = new List<ActionEffect>
                        {
                            new ActionEffect
                            {
                                EffectType = EffectType.GainInfluence,
                                TargetType = EffectTargetType.Self,
                                FixedValue = 4,
                                Order = 1
                            }
                        }
                    },
                    // Azione attiva: cura eroe
                    new BuildingActionTemplate
                    {
                        Name = "Healing Prayer",
                        Description = "Heal an assigned hero",
                        ActionType = ActionType.Active,
                        MaxHeroSlots = 1,
                        Effects = new List<ActionEffect>
                        {
                            new ActionEffect
                            {
                                EffectType = EffectType.HealHero,
                                TargetType = EffectTargetType.Self,
                                MinValue = 5,
                                MaxValue = 10,
                                StatAffected = HeroStatsEnumeration.LifePoints,
                                Order = 1
                            }
                        }
                    }
                }
            };
        }

        // Aggiungi altri template...
    }
}