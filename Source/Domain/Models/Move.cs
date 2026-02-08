using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models
{

    public class Move
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Tipo di mossa
        public MoveType MoveType { get; set; }

        // Proprietà comuni
        public int Damage { get; set; }
        public int ManaCost { get; set; }

        // Proprietà per SimpleMove (nullable se non applicabile)
        public TargetType? SimpleTargetMove { get; set; }
        public HeroStatsEnumeration? StatToHit { get; set; }
        public int? NumberTargets { get; set; }
        public int? Range { get; set; }

        // Proprietà per AreaMove (nullable se non applicabile)
        public TargetType? AreaTargetMove { get; set; }

        // Per SelfMove non servono proprietà aggiuntive

        // Foreign key opzionale per HeroClass
        public int? HeroClassId { get; set; }
        public HeroClassType? HeroClass { get; set; }

        public List<MoveTypeStrategy> Strategies { get; set; } = new();


        // Se vuoi salvare le strategie su DB, usa una stringa JSON
        public string? StrategiesJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    
    public enum MoveType
    {
        Simple,
        Area,
        Self
    }

    public enum TargetType
    {
        Allies,
        Enemies,
        All
    }
}


