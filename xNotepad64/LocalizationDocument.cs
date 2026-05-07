using System.Globalization;

namespace xNotepad64
{
    public sealed class LocalizationDocument
    {
        private readonly Dictionary<string, string> _strings;

        private LocalizationDocument(
            string filePath,
            string id,
            string title,
            string cultureName,
            string version,
            string author,
            string description,
            Dictionary<string, string> strings)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Id = id;
            Title = title;
            CultureName = cultureName;
            Version = version;
            Author = author;
            Description = description;
            _strings = strings;
        }

        public string FilePath { get; }

        public string FileName { get; }

        public string Id { get; }

        public string Title { get; }

        public string CultureName { get; }

        public string Version { get; }

        public string Author { get; }

        public string Description { get; }

        public IReadOnlyDictionary<string, string> Strings => _strings;

        public CultureInfo GetCultureOrDefault()
        {
            try
            {
                return string.IsNullOrWhiteSpace(CultureName)
                    ? CultureInfo.InvariantCulture
                    : CultureInfo.GetCultureInfo(CultureName);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture;
            }
        }

        public string GetString(string key, string fallback)
        {
            return _strings.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : fallback;
        }

        public static LocalizationDocument Load(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var strings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string section = string.Empty;

            foreach (string rawLine in File.ReadLines(filePath))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#'))
                {
                    continue;
                }

                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    section = line[1..^1].Trim();
                    continue;
                }

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                string key = line[..separatorIndex].Trim();
                string value = UnescapeValue(line[(separatorIndex + 1)..].Trim());

                if (section.Equals("meta", StringComparison.OrdinalIgnoreCase))
                {
                    metadata[key] = value;
                }
                else if (section.Equals("strings", StringComparison.OrdinalIgnoreCase))
                {
                    strings[key] = value;
                }
            }

            string fileName = Path.GetFileName(filePath);
            string id = GetValue(metadata, "id", Path.GetFileNameWithoutExtension(fileName));
            string title = GetValue(metadata, "title", fileName);
            string cultureName = GetValue(metadata, "culture", string.Empty);
            string version = GetValue(metadata, "version", string.Empty);
            string author = GetValue(metadata, "author", string.Empty);
            string description = GetValue(metadata, "description", string.Empty);

            return new LocalizationDocument(filePath, id, title, cultureName, version, author, description, strings);
        }

        private static string GetValue(IReadOnlyDictionary<string, string> values, string key, string fallback)
        {
            return values.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : fallback;
        }

        private static string UnescapeValue(string value)
        {
            var builder = new System.Text.StringBuilder(value.Length);

            for (int index = 0; index < value.Length; index++)
            {
                char current = value[index];
                if (current != '\\' || index == value.Length - 1)
                {
                    builder.Append(current);
                    continue;
                }

                index++;
                char escape = value[index];

                builder.Append(escape switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    _ => escape
                });
            }

            return builder.ToString();
        }
    }
}
