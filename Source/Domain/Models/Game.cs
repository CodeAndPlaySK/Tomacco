using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models
{

   
    
    public class Game
    {
        [Key]
        [MaxLength(10)]
        public string Code { get; set; }

        public string PlayerCreatorId { get; set; }
        public Player PlayerCreator { get; set; }

        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }

        public List<Player> Players { get; set; }

        // Famiglie nel gioco
        public List<Family> Families { get; set; } = new();

        // Relazione 1-to-1 con City
        public int CityId { get; set; }
        public City City { get; set; }
        public GameState State { get; set; } = GameState.NotStarted;

        // Configurazione
        public int InitialGold { get; set; } = 100;
        public int InitialInfluence { get; set; } = 10;
        public int CitySlots { get; set; } = 12;

        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public int CurrentTurn { get; set; }
        public List<GameTurn> Turns { get; set; }
    }
}
