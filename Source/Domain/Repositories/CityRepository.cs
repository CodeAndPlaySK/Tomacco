using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICityRepository
    {
        public ICity? GetById(int id);

        public void InsertCity(ICity city);

        public List<ICity> GetAll();
    }

    public class CityRepositoryDyummy : ICityRepository
    {
        private readonly Dictionary<int, ICity> _cities = [];
        public ICity? GetById(int id) => _cities.GetValueOrDefault(id);

        public void InsertCity(ICity city)
        {
            _cities.Add(city.Id, city);
        }

        public List<ICity> GetAll()
        {
            return _cities.Values.ToList();
        }
    }
}
