using Tomacco.Source.Entities;

namespace Domain.Entities
{
    public interface IBuilding
    {
        int Id { get; set; }
        IFamily Owner { get; set; }
        ISlotCityBuilding SlotCity { get; set; }
        IBuildingTemplate Template { get; set; }
        List<ISlotActionBuilding> SlotActions { get; set; }
    }

    public class Building : IBuilding
    {
        public int Id { get; set; }
        public IFamily Owner { get; set; }
        public ISlotCityBuilding SlotCity { get; set; }
        public IBuildingTemplate Template { get; set; }
        public List<ISlotActionBuilding> SlotActions { get; set; }
    }
}
