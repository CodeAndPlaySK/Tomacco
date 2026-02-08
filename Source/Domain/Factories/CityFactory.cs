using Domain.Models;

namespace Domain.Factories
{
    public interface ICityFactory
    {
        City CreateCity(string gameCode, int numberOfSlots = 12);
    }

    public class CityFactory : ICityFactory
    {
        private static readonly string[] CityNames =
        {
            "Valdoria", "Crystalheim", "Ironforge", "Silvermoon", "Stormwind",
            "Ravenholdt", "Goldshire", "Moonbrook", "Lakeshire", "Darkshire",
            "Westfall", "Redridge", "Duskwood", "Elwynn", "Northshire"
        };

        private static readonly string[] SlotNames =
        {
            "Piazza Centrale", "Quartiere Nord", "Quartiere Sud", "Quartiere Est",
            "Quartiere Ovest", "Porto", "Collina del Castello", "Mercato Vecchio",
            "Via dei Mercanti", "Giardini Reali", "Distretto Artigiano", "Zona Mineraria",
            "Bosco Sacro", "Riva del Fiume", "Torre di Guardia", "Antico Tempio"
        };

        private readonly Random _random = new();

        public City CreateCity(string gameCode, int numberOfSlots = 12)
        {
            var cityName = CityNames[_random.Next(CityNames.Length)];

            var city = new City
            {
                Name = cityName,
                Description = $"La gloriosa città di {cityName}",
                GameCode = gameCode,
                MaxSlots = numberOfSlots,
                Slots = new List<SlotBuilding>()
            };

            // Crea gli slot vuoti
            for (int i = 0; i < numberOfSlots; i++)
            {
                var slotName = i < SlotNames.Length
                    ? SlotNames[i]
                    : $"Zona {i + 1}";

                city.Slots.Add(new SlotBuilding
                {
                    Name = slotName,
                    Position = i + 1,
                    City = city
                });
            }

            return city;
        }
    }
}
