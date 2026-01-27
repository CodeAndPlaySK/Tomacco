using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ICityNameGenerator
    {
        string GenerateName();
    }
    public class CityNameGenerator : ICityNameGenerator
    {
        public string GenerateName()
        {
            var names = new string[] { "Samarcanda", "Peloponneso", "Artriv" };
            return names[new Random().Next(3)];
        }
    }
}
