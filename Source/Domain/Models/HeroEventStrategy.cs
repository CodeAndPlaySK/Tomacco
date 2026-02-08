using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public interface IHeroEventStrategy
    {
        string Name { get; set; }
    }

    public class HeroEventStrategy : IHeroEventStrategy
    {
        public string Name { get; set; }
    }

    public interface IResourceFamilyManagingHeroEventStrategy : IHeroEventStrategy
    {
        FamilyResourceEnum Resource { get; set; }
        Func<Hero, int> Amount { get; set; }
    }

    public class ResourceFamilyManagingHeroEventStrategy : HeroEventStrategy, IResourceFamilyManagingHeroEventStrategy
    {
        public FamilyResourceEnum Resource { get; set; }
        public Func<Hero, int> Amount { get; set; }
    }

}
