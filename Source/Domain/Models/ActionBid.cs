using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ActionBid
    {
        public int Id { get; set; }

        // Riferimento allo slot
        public int ActiveActionSlotId { get; set; }
        public ActiveActionSlot ActiveActionSlot { get; set; } = null!;

        // Chi fa l'offerta
        public int FamilyId { get; set; }
        public Family Family { get; set; } = null!;

        // Eroe assegnato
        public int HeroId { get; set; }
        public Hero Hero { get; set; } = null!;

        // Offerta di influenza
        public int InfluenceBid { get; set; }

        // Stato della bid
        public bool IsWinner { get; set; } = false;
        public bool IsResolved { get; set; } = false;

        // Turno di riferimento
        public int TurnNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
