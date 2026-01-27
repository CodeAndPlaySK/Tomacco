using Tomacco.Source.Entities;

namespace Domain.Entities.Factories
{
    public interface IBuildingFactory
    {
        IBuilding CreateBuilding(IBuildingTemplate buildingTemplate, int id, IFamily familyOwner, ISlotCityBuilding slotBuilding);
    }

    public class BuildingFactory : IBuildingFactory
    {
        public IBuilding CreateBuilding(IBuildingTemplate buildingTemplate, int id, IFamily familyOwner, ISlotCityBuilding slotBuilding)
        {
            return new Building
            {
                Id = id,
                Owner = familyOwner,
                SlotActions = [],
                SlotCity = slotBuilding,
                Template = buildingTemplate
            };
        }
    }


}
