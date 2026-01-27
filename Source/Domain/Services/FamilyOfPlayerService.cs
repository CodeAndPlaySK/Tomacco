using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Tomacco.Source.Entities;

namespace Domain.Services
{
    public interface IFamilyOfPlayerService
    {
        List<IFamilyOfPlayer> CreateFamiliesFromPlayers(List<IPlayer> players);
        
    }

    public class FamilyOfPlayerService : IFamilyOfPlayerService
    {
        private readonly IFamilyOfPlayerRepository _repository;

        public FamilyOfPlayerService(IFamilyOfPlayerRepository repository)
        {
            _repository = repository;
        }

        public List<IFamilyOfPlayer> CreateFamiliesFromPlayers(List<IPlayer> players)
        {
            return players.AsParallel().Select(_CreateFamily).ToList();
        }

        private IFamilyOfPlayer _CreateFamily(IPlayer player)
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
