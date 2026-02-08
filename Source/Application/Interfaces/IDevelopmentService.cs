using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IDevelopmentService
    {
        (Hero[], Move[]) GenerateTestMainEntities();

        FamilyOfPlayer[] CreateRandomStartingFamilyOfPlayers(int numberPlayers);
        Player[] CreateRandomPlayerForGame(int number);
        Family[] CreateRandomStartingFamilies(int familiesNumber);
        List<Hero> CreateRandomFirstLevelHeroes(Family[] families, (HeroClassType[] heroClasses, Move[] moves) mainEntities, int heroesNumber);
        void PrintHeroesStatsOnConsole(List<Hero> heroes, bool isSortedByFamily);
    }
}
