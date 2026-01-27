using Tomacco.Source.Entities;

namespace Domain.Entities
{
    public interface IBuildingAction
    {
        int Id { get; set; }
        List<IHeroEventStrategy> Events { get; set; }
    }
    public class BuildingAction : IBuildingAction
    {
        public int Id { get; set; }
        public List<IHeroEventStrategy> Events { get; set; }
    }
}
