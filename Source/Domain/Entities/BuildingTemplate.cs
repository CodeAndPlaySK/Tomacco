using Domain.Entities;

namespace Tomacco.Source.Entities
{
    public interface IBuildingTemplate
    {
        IBuildingKind Kind { get; init; }
        int Cost { get; set; }
        List<IBuildingAction> Actions { get; init; }
        List<IEventStrategy> PassiveEvents { get; set; }
    }

    public class BuildingTemplate : IBuildingTemplate
    {
        public IBuildingKind Kind { get; init; }
        public int Cost { get; set; }
        public List<IBuildingAction> Actions { get; init; }
        public List<IEventStrategy> PassiveEvents { get; set; }
    }
}
