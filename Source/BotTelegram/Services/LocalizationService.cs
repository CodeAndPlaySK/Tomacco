using System.Text.Json;
using Application.Interfaces;

namespace TelegramBot.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizations = new();
        private readonly string[] _supportedLanguages = { "it", "en", "de" };

        public LocalizationService()
        {
            _LoadLocalizations();
        }

        private void _LoadLocalizations()
        {
            foreach (var lang in _supportedLanguages)
            {
                var filePath = Path.Combine("Localization", "Resources", $"messages.{lang}.json");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  File di localizzazione non trovato: {filePath}");
                    continue;
                }

                try
                {
                    var json = File.ReadAllText(filePath);
                    var messages = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (messages != null)
                    {
                        _localizations[lang] = messages;
                        Console.WriteLine($"✅ Localizzazione caricata: {lang}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Errore nel caricamento di {filePath}: {ex.Message}");
                }
            }
        }

        public string GetString(string key, string languageCode, params object[] args)
        {
            if (!_localizations.ContainsKey(languageCode))
            {
                languageCode = "en";
            }

            if (!_localizations[languageCode].ContainsKey(key))
            {
                Console.WriteLine($"⚠️  Chiave di localizzazione mancante: {key} per lingua {languageCode}");
                return key;
            }

            var message = _localizations[languageCode][key];

            if (args.Length > 0)
            {
                try
                {
                    return string.Format(message, args);
                }
                catch
                {
                    return message;
                }
            }

            return message;
        }

        public string GetLanguageName(string languageCode) =>
            languageCode switch
            {
                "it" => "🇮🇹 Italiano",
                "en" => "🇬🇧 English",
                "de" => "🇩🇪 Deutsch",
                _ => "🇬🇧 English"
            };

        public bool IsLanguageSupported(string languageCode) => _supportedLanguages.Contains(languageCode);

        public string[] GetSupportedLanguages() => _supportedLanguages;
    }
}
