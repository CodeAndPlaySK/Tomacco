
namespace Domain.Models
{

    public class Family 
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoatOfArms { get; set; } = string.Empty; // Emoji o simbolo

        // Riferimento al Game
        public string GameCode { get; set; } = string.Empty;
        public Game Game { get; set; } = null!;

        // Membri della famiglia
        public List<FamilyOfPlayer> Members { get; set; } = new();

        public int ResourcesId { get; set; }
        public FamilyResources Resources { get; set; }
        
        // Eroi della famiglia
        public List<Hero> Heroes { get; set; } = new();


        // Edifici posseduti
        public List<Building> Buildings { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum FamilyResourceEnum
    {
        Gold, Influence
    }

    public class FamilyResources 
    {
        public int Id { get; set; }
        public int Gold { get; set; }
        public int Influence { get; set; }
    }
}
