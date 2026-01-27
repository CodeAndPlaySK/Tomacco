using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ConsoleApp
{
    public interface IMenuPageResolver
    {
        public void DisplayMenu();
    }
    public class MenuPageResolver : IMenuPageResolver
    {
        private readonly IMenuPageRegistry _registry;
        private readonly Stack<IMenuPage> _openedPages;

        public MenuPageResolver(IMenuPageRegistry registry)
        {
            _registry = registry;
            _openedPages = new Stack<IMenuPage>();
        }

        public void DisplayMenu()
        {
            var page = _registry.GetRootPage();
            _openedPages.Clear();
            _openedPages.Push(page);

            while (true)
            {
                page.DisplayPage();
                if (page.OtherPages.Count == 0)
                {
                    if (_openedPages.Count > 1)
                    {
                        _openedPages.Pop();
                        page = _openedPages.Peek();
                        continue;
                    }
                    else
                    {
                        // No more pages to go back to, exit or show error
                        return;
                    }
                }

                var input = Console.ReadLine();
                if (input == null)
                {
                    _registry.Resolve("Invalid Menu").DisplayPage();
                    continue;
                }
                input = input.Trim().ToUpper();
                if (string.IsNullOrEmpty(input))
                {
                    _registry.Resolve("Invalid Menu").DisplayPage();
                    continue;
                }

                if (input is "BACK" or "B")
                {
                    if (_openedPages.Count > 1)
                    {
                        _openedPages.Pop();
                        page = _openedPages.Peek();
                        continue;
                    }
                    else
                    {
                        // No more pages to go back to, exit or show error
                        return;
                    }
                }

                var newPage = page.OtherPages.FirstOrDefault(kv => kv.Key.Contains(input)).Value;
                if (newPage == null)
                {
                    _registry.Resolve("Invalid Menu").DisplayPage();
                    continue;
                }
                _openedPages.Push(newPage);
                page = newPage;
            }
        }
    }
}
