using System.Globalization;

namespace xNotepad64
{
    public static class LocalizationManager
    {
        private static readonly StringComparer FileNameComparer = StringComparer.OrdinalIgnoreCase;
        private static IReadOnlyList<LocalizationDocument> _availableLanguages = [];

        public static string LocalizationDirectoryPath =>
            Path.Combine(AppContext.BaseDirectory, "Localization");

        public static IReadOnlyList<LocalizationDocument> AvailableLanguages => _availableLanguages;

        public static LocalizationDocument? CurrentLanguage { get; private set; }

        public static void Initialize(string? preferredLanguageFileName)
        {
            ReloadAvailableLanguages();
            SetCurrentLanguage(preferredLanguageFileName);
        }

        public static IReadOnlyList<LocalizationDocument> ReloadAvailableLanguages()
        {
            var languages = new List<LocalizationDocument>();

            if (Directory.Exists(LocalizationDirectoryPath))
            {
                foreach (string filePath in Directory.GetFiles(LocalizationDirectoryPath, "*.local", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        languages.Add(LocalizationDocument.Load(filePath));
                    }
                    catch
                    {
                        // Ungueltige Sprachdateien werden uebersprungen.
                    }
                }
            }

            _availableLanguages = languages
                .OrderBy(language => language.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            return _availableLanguages;
        }

        public static LocalizationDocument? SetCurrentLanguage(string? preferredLanguageFileName)
        {
            if (_availableLanguages.Count == 0)
            {
                CurrentLanguage = null;
                ApplyCulture(CultureInfo.InvariantCulture);
                return null;
            }

            LocalizationDocument selectedLanguage = FindLanguage(preferredLanguageFileName)
                ?? FindLanguage(EditorSettings.DefaultLanguageFileName)
                ?? _availableLanguages[0];

            CurrentLanguage = selectedLanguage;
            ApplyCulture(selectedLanguage.GetCultureOrDefault());
            return selectedLanguage;
        }

        public static string Get(string key, string fallback)
        {
            return CurrentLanguage?.GetString(key, fallback) ?? fallback;
        }

        public static string Format(string key, string fallback, params object[] arguments)
        {
            string template = Get(key, fallback);
            return string.Format(CultureInfo.CurrentCulture, template, arguments);
        }

        private static LocalizationDocument? FindLanguage(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return _availableLanguages.FirstOrDefault(language => FileNameComparer.Equals(language.FileName, fileName));
        }

        private static void ApplyCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
