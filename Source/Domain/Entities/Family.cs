using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomacco.Source.Entities
{
    public interface IFamily
    {
        int Id { get; init; }
        string Name { get; set; }
        IFamilyResources Resources { get; set; }

        List<IHero> Heroes { get; set; }
    }

    public class Family : IFamily
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public IFamilyResources Resources { get; set; }
        public List<IHero> Heroes { get; set; }
    }

    public enum FamilyResourceEnum
    {
        Gold, Influence
    }

    public interface IFamilyResources
    {
        int Gold { get; set; }
        int Influence { get; set; }
    }

    public class FamilyResources : IFamilyResources
    {
        public int Gold { get; set; }
        public int Influence { get; set; }
    }
}
