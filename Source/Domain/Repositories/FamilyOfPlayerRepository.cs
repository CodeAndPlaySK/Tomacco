using Domain.Entities;
using Tomacco.Source.Entities;

namespace Domain.Repositories
{
    public interface IFamilyOfPlayerRepository
    {
        IFamilyOfPlayer? GetByIdFamilyAndTelegramIdPlayer(int idFamily, string telegramIdPlayer);
        void InsertFamilyOfPlayer(IFamilyOfPlayer familyOfPlayer);
        public List<IFamilyOfPlayer> GetAll();
    }

    public class FamilyOfPlayerRepositoryDummy : IFamilyOfPlayerRepository
    {
        private readonly Dictionary<(int idFamily, string telegramIdPlayer), IFamilyOfPlayer> _familyOfPlayers = [];

        public IFamilyOfPlayer? GetByIdFamilyAndTelegramIdPlayer(int idFamily, string telegramIdPlayer)
        {
            return _familyOfPlayers[(idFamily, telegramIdPlayer)];
        }

        public void InsertFamilyOfPlayer(IFamilyOfPlayer familyOfPlayer)
        {
            _familyOfPlayers.Add((familyOfPlayer.Family.Id, familyOfPlayer.Player.TelegramId), familyOfPlayer);
        }

        public List<IFamilyOfPlayer> GetAll()
        {
            return _familyOfPlayers.Values.ToList();
        }
    }
}
