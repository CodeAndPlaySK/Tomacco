namespace Domain.Models
{
    public enum HeroStatsEnumeration
    {
        LifePoints, 
        MoralityPoints, 
        Physic, 
        Mind, 
        Faith, 
        Speed, 
        Defence,
        Charisma,
        Level
    }

    public class HeroStats
    {
        public int Id { get; set; }
        public BarPoints LifePoints { get; set; }
        public BarPoints MoralityPoints { get; set; }
        public int Level { get; set; }
        public int Physic { get; set; }
        public int Mind { get; set; }
        public int Faith { get; set; }
        public int Speed { get; set; }
        public int Charisma { get; set; }
        public int Defence { get; set; }
    }
}
