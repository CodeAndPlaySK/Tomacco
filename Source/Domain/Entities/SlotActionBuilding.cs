using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Tomacco.Source.Entities
{
    public interface ISlotActionBuilding
    {
        IBuilding Building { get; set; }
        IBuildingAction Action { get; set; }
        IHero HeroAssigned { get; set; }
}
    public class SlotActionBuilding : ISlotActionBuilding
    {
        public IBuilding Building { get; set; }
        public IBuildingAction Action { get; set; }
        public IHero HeroAssigned { get; set; }
    }
}
