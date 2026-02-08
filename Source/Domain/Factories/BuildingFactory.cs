using Domain.Models;

namespace Domain.Factories
{
    public interface IBuildingFactory
    {
        Building CreateBuilding(BuildingTemplate buildingTemplate, int id, Family familyOwner, SlotBuilding slotBuilding);
    }

    public class BuildingFactory : IBuildingFactory
    {
        public Building CreateBuilding(BuildingTemplate buildingTemplate, int id, Family familyOwner, SlotBuilding slotBuilding)
        {
            return new Building
            {
                Id = id,
                FamilyOwner = familyOwner,
                Template = buildingTemplate
            };
        }
    }


}
