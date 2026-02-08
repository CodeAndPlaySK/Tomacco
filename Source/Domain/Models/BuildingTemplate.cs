
using Domain.Enums;

namespace Domain.Models
{

    public class BuildingTemplate 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public BuildingKindType KindType { get; init; }
        public int CostructionCost { get; set; }
        public int TurnsToComplete { get; set; } = 1;
        public List<BuildingActionTemplate> ActionTemplates { get; init; }
    }
}
