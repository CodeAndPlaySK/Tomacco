using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
  

    public class FamilyOfPlayerService : IFamilyOfPlayerService
    {
        public List<FamilyOfPlayer> CreateFamiliesFromPlayers(List<Player> players)
        {
            return players.AsParallel().Select(_CreateFamily).ToList();
        }

        private FamilyOfPlayer _CreateFamily(Player player)
        {
            return new FamilyOfPlayer
            {
                Player = player,
                Family = new Family
                {
                    Id = new Random().Next(),
                    Heroes = [],
                    Name = player.Username,
                    Resources = new FamilyResources
                    {
                        Gold = 50,
                        Influence = 0
                    }
                }
            };
        }
    }
}
