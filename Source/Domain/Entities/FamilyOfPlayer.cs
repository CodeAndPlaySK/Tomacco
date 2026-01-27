using Tomacco.Source.Entities;

namespace Domain.Entities
{
    public interface IFamilyOfPlayer
    {
        IPlayer Player { get; set; }
        IFamily Family { get; set; }
    }

    public class FamilyOfPlayer : IFamilyOfPlayer
    {
        public IPlayer Player { get; set; }
        public IFamily Family { get; set; }
    }
}
