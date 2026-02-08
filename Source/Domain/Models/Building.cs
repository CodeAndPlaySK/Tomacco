
namespace Domain.Models
{

    public class Building
    {
        public int Id { get; set; }
        public string? CustomName { get; set; } // Nome personalizzato opzionale


        public int FamilyOwnerId { get; set; }
        public Family FamilyOwner { get; set; }

        // Stato costruzione
        public bool IsCompleted { get; set; } = false;
        public int TurnsRemaining { get; set; }


        // Template di riferimento
        public int TemplateId { get; set; }
        public BuildingTemplate Template { get; set; }

        // Slot delle azioni attive (dove assegnare gli eroi)
        public List<ActiveActionSlot> ActiveActionSlots { get; set; } = new();

        public DateTime BuiltAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Helper
        public string Name => CustomName ?? Template?.Name ?? "Unknown";
    }
}
