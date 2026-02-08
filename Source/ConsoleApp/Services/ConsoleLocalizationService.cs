using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Console.Services
{
    public class ConsoleLocalizationService : ILocalizationService
    {
        public string GetString(string key, string languageCode, params object[] args)
        {
            // Versione semplificata per console
            return key;
        }

        public string GetLanguageName(string languageCode)
        {
            return languageCode switch
            {
                "it" => "🇮🇹 Italiano",
                "en" => "🇬🇧 English",
                "de" => "🇩🇪 Deutsch",
                _ => "Unknown"
            };
        }

        public bool IsLanguageSupported(string languageCode)
        {
            return languageCode is "it" or "en" or "de";
        }

        public string[] GetSupportedLanguages()
        {
            return new[] { "it", "en", "de" };
        }
    }
}
