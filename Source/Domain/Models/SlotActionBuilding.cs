namespace Domain.Models
{
    public class SlotActionBuilding 
    {
        public int idBuilding { get; set; }
        public Building Building { get; set; }

        public int idAction { get; set; }
        public BuildingActionTemplate ActionTemplate { get; set; }

        public int idHeroAssigned { get; set; }
        public Hero HeroAssigned { get; set; }
    }
}
