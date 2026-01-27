using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public interface IMenuPageRegistry
    {
        IMenuPage GetRootPage();
        void Register(IMenuPage menuPage);
        IMenuPage Resolve(string name);
    }
    public class MenuPageRegistry : IMenuPageRegistry
    {
        private readonly Dictionary<string, IMenuPage> _menuPages;

        public MenuPageRegistry()
        {
            _menuPages = new Dictionary<string, IMenuPage>();
        }

        public void Register(IMenuPage menuPage)
        {
            _menuPages.Add(menuPage.Name, menuPage);
        }

        public IMenuPage Resolve(string name)
        {
            return _menuPages[name];
        }

        public IMenuPage GetRootPage()
        {
            return _menuPages.Select(pair=>pair.Value).First(page => page.IsRootPage);
        }
    }
}
