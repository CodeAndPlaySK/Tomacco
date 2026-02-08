using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IActionBidService
    {
        Task<ActionBid> PlaceBidAsync(int slotId, int familyId, int heroId, int influenceAmount, int turnNumber);
        Task<List<ActionBid>> GetBidsForSlotAsync(int slotId, int turnNumber);
        Task ResolveBidsForTurnAsync(string gameCode, int turnNumber);
    }
}
