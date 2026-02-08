using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Models
{
    public class ActionCondition
    {
        public int Id { get; set; }

        public int ActionTemplateId { get; set; }
        public BuildingActionTemplate ActionTemplate { get; set; } = null!;

        public ConditionType ConditionType { get; set; }

        // Parametri della condizione
        public int? RequiredAmount { get; set; }
        public HeroClassType? RequiredHeroClass { get; set; }
        public BuildingKindType? RequiredBuildingKind { get; set; }
    }
}
