namespace Domain.Models
{
    
    public class SlotBuilding 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // es. "Slot Nord", "Piazza Centrale"
        public int Position { get; set; } // Ordine/posizione nello slot

        public int CityId { get; set; }
        public City City { get; set; }

        // Building costruito (null se vuoto)
        public int? BuildingId { get; set; }
        public Building? Building { get; set; }

        // Helpers
        public bool IsEmpty => BuildingId == null;
        public bool HasBuilding => BuildingId != null;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
