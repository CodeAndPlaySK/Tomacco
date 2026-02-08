
namespace Domain.Models
{
    public class City 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;

        // Relazione 1-to-1 con Game
        public string GameCode { get; set; }
        public Game Game { get; set; }
        
        // Slot disponibili per costruire
        public List<SlotBuilding> Slots { get; set; } = new();
        
        // Configurazione città
        public int MaxSlots { get; set; } = 12;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
