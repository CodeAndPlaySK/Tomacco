
namespace Domain.Models
{

    public class FamilyOfPlayer     {
        public Player Player { get; set; }
        public string PlayerTelegramId { get; set; }

        public Family Family { get; set; }
        public int FamilyId { get; set; }

        public bool IsOwner { get; set; } = false; // Il "capo famiglia"
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
