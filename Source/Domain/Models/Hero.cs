using Domain.Enums;

namespace Domain.Models
{
    public class Hero 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FamilyId { get; set; }
        public Family Family { get; set; }
        public HeroClassType HeroClassType { get; set; }
        public HeroStats Stats { get; set; }
        public List<Move> Moves { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
