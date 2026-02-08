using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Factories
{
    public interface IFamilyFactory
    {
        Family CreateFamily(string gameCode, Player player, int initialGold = 100, int initialInfluence = 10);
    }

    public class FamilyFactory : IFamilyFactory
    {
        private static readonly string[] FamilyNamePrefixes =
        {
            "von", "de", "della", "van", "di", "du", "le", "la"
        };

        private static readonly string[] FamilyNameRoots =
        {
            "Rossi", "Bianchi", "Neri", "Verdi", "Azzurri",
            "Leone", "Aquila", "Drago", "Lupo", "Orso",
            "Ferro", "Oro", "Argento", "Bronzo", "Acciaio",
            "Monte", "Valle", "Fiume", "Bosco", "Campo",
            "Sole", "Luna", "Stella", "Tempesta", "Fulmine"
        };

        private static readonly string[] CoatOfArms =
        {
            "🦁", "🦅", "🐉", "🐺", "🐻", "🦊", "🦌", "🐎",
            "⚔️", "🛡️", "👑", "🏰", "⭐", "🌙", "☀️", "🔥"
        };

        private readonly Random _random = new();

        public Family CreateFamily(string gameCode, Player player, int initialGold = 100, int initialInfluence = 10)
        {
            var familyName = GenerateFamilyName(player.Username);
            var coatOfArms = CoatOfArms[_random.Next(CoatOfArms.Length)];

            var family = new Family
            {
                Name = familyName,
                Description = $"La nobile casata {familyName}",
                CoatOfArms = coatOfArms,
                GameCode = gameCode,
                Resources = new FamilyResources
                {
                    Gold = initialGold,
                    Influence = initialInfluence
                },
                Members = new List<FamilyOfPlayer>()
            };

            // Aggiungi il player come owner della famiglia
            family.Members.Add(new FamilyOfPlayer
            {
                Family = family,
                PlayerTelegramId = player.TelegramId,
                Player = player,
                IsOwner = true
            });

            return family;
        }

        private string GenerateFamilyName(string playerName)
        {
            var usePrefix = _random.Next(2) == 0;
            var prefix = usePrefix ? FamilyNamePrefixes[_random.Next(FamilyNamePrefixes.Length)] + " " : "";
            var root = FamilyNameRoots[_random.Next(FamilyNameRoots.Length)];

            // A volte usa parte del nome del giocatore
            if (_random.Next(3) == 0 && playerName.Length >= 3)
            {
                var playerPart = char.ToUpper(playerName[0]) + playerName.Substring(1, Math.Min(playerName.Length - 1, 4)).ToLower();
                return $"{prefix}{playerPart}{root.ToLower()}i";
            }

            return $"{prefix}{root}";
        }
    }
}
