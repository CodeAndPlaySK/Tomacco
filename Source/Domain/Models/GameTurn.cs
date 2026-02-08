using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{

    public class GameTurn 
    {

        public int Id { get; set; }
        public string GameCode { get; set; }
        public Game Game { get; set; }
        public int TurnNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
