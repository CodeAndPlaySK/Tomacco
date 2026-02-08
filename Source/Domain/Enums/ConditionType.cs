using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ConditionType
    {
        None,
        HasHeroOfClass,     // Possiede almeno N eroi di una classe
        HasBuilding,        // Possiede un certo edificio
        HasMinGold,         // Ha almeno N oro
        HasMinInfluence,    // Ha almeno N influenza
        HasMinHeroes        // Ha almeno N eroi
    }
}
