using Tomacco.Source.Entities;

namespace Domain.Entities
{
    public interface ICity
    {
        int Id { get; set; }
        List<IFamilyOfPlayer> FamilyOfPlayers { get; set; }
        ISlotCityBuilding[] SlotCityBuildings { get; set; }
    }
    public class City : ICity
    {
        public int Id { get; set; }
        public List<IFamilyOfPlayer> FamilyOfPlayers { get; set; }
        public ISlotCityBuilding[] SlotCityBuildings { get; set; }
    }
}
