using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomacco.Source.Entities
{
    public interface IHero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFamily Family { get; set; }
        public IHeroClass Class { get; set; }
        public IHeroStats Stats { get; set; }
        public List<IMove> Moves { get; set; }
    }
    public class Hero : IHero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFamily Family { get; set; }
        public IHeroClass Class { get; set; }
        public IHeroStats Stats { get; set; }
        public List<IMove> Moves { get; set; }
    }
}
