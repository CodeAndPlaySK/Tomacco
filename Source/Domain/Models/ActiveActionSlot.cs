using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ActiveActionSlot
    {
        public int Id { get; set; }

        // Riferimento all'edificio
        public int BuildingId { get; set; }
        public Building Building { get; set; } = null!;

        // Riferimento al template dell'azione
        public int ActionTemplateId { get; set; }
        public BuildingActionTemplate ActionTemplate { get; set; } = null!;

        // Prenotazioni dei giocatori per questo turno
        public List<ActionBid> Bids { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
