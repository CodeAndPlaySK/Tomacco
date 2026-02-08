using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ActionBidService : IActionBidService
    {
        private readonly GameDbContext _context;

        public ActionBidService(GameDbContext context)
        {
            _context = context;
        }

        public async Task<ActionBid> PlaceBidAsync(int slotId, int familyId, int heroId, int influenceAmount, int turnNumber)
        {
            // Verifica che l'eroe non sia già assegnato ad altre azioni in questo turno
            var existingBid = await _context.ActionBids
                .AnyAsync(b => b.HeroId == heroId && b.TurnNumber == turnNumber && !b.IsResolved);

            if (existingBid)
            {
                throw new InvalidOperationException("This hero is already assigned to another action this turn");
            }

            // Verifica che la famiglia non abbia già fatto bid su questo slot
            var existingSlotBid = await _context.ActionBids
                .AnyAsync(b => b.ActiveActionSlotId == slotId && b.FamilyId == familyId && b.TurnNumber == turnNumber);

            if (existingSlotBid)
            {
                throw new InvalidOperationException("You already have a bid on this action slot");
            }

            var bid = new ActionBid
            {
                ActiveActionSlotId = slotId,
                FamilyId = familyId,
                HeroId = heroId,
                InfluenceBid = influenceAmount,
                TurnNumber = turnNumber
            };

            _context.ActionBids.Add(bid);
            await _context.SaveChangesAsync();

            return bid;
        }

        public async Task<List<ActionBid>> GetBidsForSlotAsync(int slotId, int turnNumber)
        {
            return await _context.ActionBids
                .Include(b => b.Family)
                .Include(b => b.Hero)
                .Where(b => b.ActiveActionSlotId == slotId && b.TurnNumber == turnNumber)
                .OrderByDescending(b => b.InfluenceBid)
                .ToListAsync();
        }

        public async Task ResolveBidsForTurnAsync(string gameCode, int turnNumber)
        {
            // Trova tutti gli slot con bid per questo turno
            var slotsWithBids = await _context.ActiveActionSlots
                .Include(s => s.Building)
                    .ThenInclude(b => b.Template)
                .Include(s => s.ActionTemplate)
                    .ThenInclude(at => at.Effects)
                .Include(s => s.Bids.Where(b => b.TurnNumber == turnNumber))
                    .ThenInclude(b => b.Family)
                .Include(s => s.Bids.Where(b => b.TurnNumber == turnNumber))
                    .ThenInclude(b => b.Hero)
                .Where(s => s.Building.Template != null && s.Bids.Any(b => b.TurnNumber == turnNumber))
                .ToListAsync();

            foreach (var slot in slotsWithBids)
            {
                var bids = slot.Bids
                    .OrderByDescending(b => b.InfluenceBid)
                    .ThenBy(b => b.CreatedAt) // In caso di parità, vince chi ha offerto prima
                    .ToList();

                if (bids.Any())
                {
                    // Il vincitore è chi ha offerto di più
                    var winner = bids.First();
                    winner.IsWinner = true;

                    // Applica gli effetti al vincitore
                    await ApplyActionEffectsAsync(winner, slot.ActionTemplate);
                }

                // Marca tutte le bid come risolte
                foreach (var bid in bids)
                {
                    bid.IsResolved = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ApplyActionEffectsAsync(ActionBid winningBid, BuildingActionTemplate actionTemplate)
        {
            foreach (var effect in actionTemplate.Effects.OrderBy(e => e.Order))
            {
                // Applica l'effetto in base al tipo
                switch (effect.EffectType)
                {
                    case EffectType.GainGold:
                        var goldAmount = CalculateEffectValue(effect);
                        winningBid.Family.Resources.Gold += goldAmount;
                        break;

                    case EffectType.GainInfluence:
                        var influenceAmount = CalculateEffectValue(effect);
                        winningBid.Family.Resources.Influence += influenceAmount;
                        break;

                        // Aggiungi altri casi...
                }
            }

            await Task.CompletedTask;
        }

        private int CalculateEffectValue(ActionEffect effect)
        {
            if (effect.FixedValue.HasValue)
                return effect.FixedValue.Value;

            if (effect.MinValue.HasValue && effect.MaxValue.HasValue)
            {
                var random = new Random();
                return random.Next(effect.MinValue.Value, effect.MaxValue.Value + 1);
            }

            return 0;
        }
    }
}
