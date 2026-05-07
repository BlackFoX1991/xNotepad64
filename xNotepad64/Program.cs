namespace xNotepad64
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            EditorSettings settings = EditorSettingsStore.Load();
            LocalizationManager.Initialize(settings.LanguageFileName);
            Application.Run(new xMain(GetStartupFilePath(args)));
        }

        private static string? GetStartupFilePath(string[] args)
        {
            foreach (string arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg) ||
                    string.Equals(arg, "/dde", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "-Embedding", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return Path.GetFullPath(arg);
            }

            return null;
        }
    }
}
