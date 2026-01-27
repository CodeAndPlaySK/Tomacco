using Domain.Entities;
using Tomacco.Source.Entities;

namespace Domain.Factories
{
    public interface ICityFactory
    {
        public ICity Create(int id, string name, IFamilyOfPlayer[] families, ISlotCityBuilding[] slotCityBuildings);
    }
    public class CityFactory : ICityFactory
    {
        public ICity Create(int id, string name, IFamilyOfPlayer[] families, ISlotCityBuilding[] slotCityBuildings)
        {
            return new City
            {
                Id = id,
                FamilyOfPlayers = families.Select(x => x).ToList(),
                SlotCityBuildings = (ISlotCityBuilding[]) slotCityBuildings.Clone()
            };
        }
    }
}
