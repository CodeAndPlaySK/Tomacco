using Tomacco.Source.Models;

namespace Tomacco.Source.Entities
{
    public enum HeroStatsEnumeration
    {
        LifePoints, MoralityPoints, Physic, Mind, Faith, Speed, Charisma
    }

    public interface IHeroStats
    {
        public IBarPoints LifePoints { get; set; }
        public IBarPoints MoralityPoints { get; set; }
        public int Level { get; set; }
        public int Physic { get; set; }
        public int Mind { get; set; }
        public int Faith { get; set; }
        public int Speed { get; set; }
        public int Charisma { get; set; }
        public int Defence { get; set; }
    }
    public class HeroStats : IHeroStats
    {
        public IBarPoints LifePoints { get; set; }
        public IBarPoints MoralityPoints { get; set; }
        public int Level { get; set; }
        public int Physic { get; set; }
        public int Mind { get; set; }
        public int Faith { get; set; }
        public int Speed { get; set; }
        public int Charisma { get; set; }
        public int Defence { get; set; }
    }
}
