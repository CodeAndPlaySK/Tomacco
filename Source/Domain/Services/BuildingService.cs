using Domain.Entities;

namespace Application.Services
{
    public interface IBuildingService
    {
        IBuilding CreateBuilding();
        IBuilding GetBuilding(int idBuilding);
    }

    public class BuildingService : IBuildingService
    {
        public IBuilding CreateBuilding()
        {
            throw new NotImplementedException();
        }

        public IBuilding GetBuilding(int idBuilding)
        {
            throw new NotImplementedException();
        }
    }
}
