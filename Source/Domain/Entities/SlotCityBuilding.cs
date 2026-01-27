using Domain.Entities;

namespace Tomacco.Source.Entities
{
    public interface ISlotCityBuilding
    {
        IBuilding Building { get; set; }
        ICity City { get; set; }
    }

    public class SlotCityBuilding : ISlotCityBuilding
    {
        public IBuilding Building { get; set; }
        public ICity City { get; set; }
    }
}
