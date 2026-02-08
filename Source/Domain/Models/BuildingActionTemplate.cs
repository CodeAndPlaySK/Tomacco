using Domain.Enums;

namespace Domain.Models
{
    public class BuildingActionTemplate 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Tipo azione (attiva o passiva)
        public ActionType ActionType { get; set; }

        // Riferimento al template dell'edificio
        public int BuildingTemplateId { get; set; }
        public BuildingTemplate BuildingTemplate { get; set; } = null!;

        // Effetti dell'azione
        public List<ActionEffect> Effects { get; set; } = new();

        // Condizioni per azioni passive (opzionali)
        public List<ActionCondition> Conditions { get; set; } = new();

        // Per azioni attive: quanti eroi possono essere assegnati contemporaneamente
        public int MaxHeroSlots { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
