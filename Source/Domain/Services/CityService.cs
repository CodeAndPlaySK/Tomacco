using Domain.Entities;
using Domain.Factories;
using Domain.Repositories;

namespace Domain.Services
{
    public interface ICityService
    {
        ICity CreateCity(string name);
        ICity? GetCity(int idCity);
    }

    public class CityService : ICityService
    {
        private readonly ICityFactory _factory;
        private readonly ICityRepository _repository;

        public CityService(ICityFactory factory, ICityRepository repository)
        {
            _factory = factory;
            _repository = repository;
        }

        public ICity CreateCity(string name)
        { 
            var newCity = _factory.Create(id: 0, name, [], []);
            _repository.InsertCity(newCity);

            return newCity;
        }

        public ICity? GetCity(int idCity)
        {
            return _repository.GetById(idCity);
        }
    }
}
