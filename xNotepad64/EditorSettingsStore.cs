using System.Text.Json;

namespace xNotepad64
{
    public static class EditorSettingsStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        private static string SettingsPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "xNotepad64", "settings.json");

        public static EditorSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    return new EditorSettings();
                }

                string json = File.ReadAllText(SettingsPath);
                EditorSettings? settings = JsonSerializer.Deserialize<EditorSettings>(json, SerializerOptions);
                settings ??= new EditorSettings();
                settings.Normalize();
                return settings;
            }
            catch
            {
                return new EditorSettings();
            }
        }

        public static void Save(EditorSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            settings.Normalize();
            EnsureSettingsDirectoryExists();

            string json = JsonSerializer.Serialize(settings, SerializerOptions);
            File.WriteAllText(SettingsPath, json);
        }

        public static async Task SaveAsync(EditorSettings settings, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(settings);

            settings.Normalize();
            EnsureSettingsDirectoryExists();

            string json = JsonSerializer.Serialize(settings, SerializerOptions);
            await File.WriteAllTextAsync(SettingsPath, json, cancellationToken);
        }

        private static void EnsureSettingsDirectoryExists()
        {
            string directory = Path.GetDirectoryName(SettingsPath) ?? AppContext.BaseDirectory;
            Directory.CreateDirectory(directory);
        }
    }
}
