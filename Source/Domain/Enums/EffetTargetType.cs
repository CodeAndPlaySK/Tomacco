using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EffectTargetType
    {
        Self, // Solo chi possiede l'edificio
        AllPlayers, // Tutti i giocatori
        Allies, // Alleati (se implementi alleanze)
        Enemies // Avversari
    }
}
