using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public interface IMenuPage
    {
        bool IsRootPage { get; init; }
        string Name { get; init; }

        Action DisplayPage { get; init; }

        Dictionary<List<string>, IMenuPage> OtherPages { get; init; }

    }
    public class MenuPage : IMenuPage
    {
        public bool IsRootPage { get; init; }
        public string Name { get; init; }
        public Action DisplayPage { get; init; }
        public Dictionary<List<string>, IMenuPage> OtherPages { get; init; }

        public MenuPage(string name, Action displayPage, bool isRootPage = false)
        {
            IsRootPage = isRootPage;
            Name = name;
            DisplayPage = displayPage;
            OtherPages = [];
        }
    }
}
