using Domain.Enums;

namespace Domain.Models
{
    public class ActionEffect
    {
        public int Id { get; set; }

        public int ActionTemplateId { get; set; }
        public BuildingActionTemplate ActionTemplate { get; set; } = null!;

        public EffectType EffectType { get; set; }
        public EffectTargetType TargetType { get; set; }

        // Parametri dell'effetto
        public int? FixedValue { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        // Per buff/effetti su eroi
        public HeroStatsEnumeration? StatAffected { get; set; }
        public HeroClassType? RequiredHeroClass { get; set; }

        public int Order { get; set; }
    }
}
