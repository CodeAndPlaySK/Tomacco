using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public enum MoveStrategyType
    {
        Attack,
        Buff,
        Heal
    }

    public class MoveTypeStrategy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MoveStrategyType StrategyType { get; set; }

        // Foreign key per Move
        public int MoveId { get; set; }
        public Move Move { get; set; } = null!;

        public int Order { get; set; } // Per ordinare le strategy

        // Parametri comuni
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? FixedValue { get; set; }

        // Per Buff
        public HeroStatsEnumeration? StatToBuff { get; set; }
        public int? NumberRounds { get; set; }

        // Per Heal
        public HeroStatsEnumeration? StatToHeal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Func calcolata a runtime (non salvata su DB)
        [NotMapped]
        public Func<int> GetValue => () => _CalculateValue();

        private int _CalculateValue()
        {
            if (FixedValue.HasValue)
                return FixedValue.Value;

            if (MinValue.HasValue && MaxValue.HasValue)
            {
                var random = new Random();
                return random.Next(MinValue.Value, MaxValue.Value + 1);
            }

            return 0;
        }
    }
}
