using System.Drawing;

namespace xNotepad64
{
    public sealed class EditorSettings
    {
        public const long BytesPerMiB = 1024L * 1024L;
        public const long BytesPerGiB = 1024L * 1024L * 1024L;
        public const long MinimumChunkSizeBytes = 1L * BytesPerMiB;
        public const long MaximumChunkSizeBytesLimit = 256L * BytesPerMiB;
        public const long DefaultChunkSizeBytes = 4L * BytesPerMiB;
        public const string DefaultTextFontFamily = "Cascadia Mono";
        public const float DefaultTextFontSize = 10F;
        public const float MinimumTextFontSize = 6F;
        public const float MaximumTextFontSize = 48F;

        public long MaximumChunkSizeBytes { get; set; } = DefaultChunkSizeBytes;

        public bool AllowAbruptChunkCutoff { get; set; }

        public string TextFontFamily { get; set; } = DefaultTextFontFamily;

        public float TextFontSize { get; set; } = DefaultTextFontSize;

        public FontStyle TextFontStyle { get; set; } = FontStyle.Regular;

        public WindowPlacementSettings? MainWindowPlacement { get; set; }

        public WindowPlacementSettings? SearchWindowPlacement { get; set; }

        public WindowPlacementSettings? OptionsWindowPlacement { get; set; }

        public EditorSettings Clone()
        {
            return new EditorSettings
            {
                MaximumChunkSizeBytes = MaximumChunkSizeBytes,
                AllowAbruptChunkCutoff = AllowAbruptChunkCutoff,
                TextFontFamily = TextFontFamily,
                TextFontSize = TextFontSize,
                TextFontStyle = TextFontStyle,
                MainWindowPlacement = MainWindowPlacement?.Clone(),
                SearchWindowPlacement = SearchWindowPlacement?.Clone(),
                OptionsWindowPlacement = OptionsWindowPlacement?.Clone()
            };
        }

        public void Normalize()
        {
            MaximumChunkSizeBytes = Math.Clamp(MaximumChunkSizeBytes, MinimumChunkSizeBytes, MaximumChunkSizeBytesLimit);

            if (string.IsNullOrWhiteSpace(TextFontFamily))
            {
                TextFontFamily = DefaultTextFontFamily;
            }

            if (float.IsNaN(TextFontSize) || float.IsInfinity(TextFontSize))
            {
                TextFontSize = DefaultTextFontSize;
            }

            TextFontSize = Math.Clamp(TextFontSize, MinimumTextFontSize, MaximumTextFontSize);
            MainWindowPlacement?.Normalize();
            SearchWindowPlacement?.Normalize();
            OptionsWindowPlacement?.Normalize();
        }

        public Font CreateTextFont()
        {
            Normalize();

            try
            {
                return new Font(TextFontFamily, TextFontSize, TextFontStyle);
            }
            catch
            {
                return new Font(FontFamily.GenericMonospace, TextFontSize, TextFontStyle);
            }
        }

        public string GetTextFontSummary()
        {
            Normalize();
            return $"{TextFontFamily}, {TextFontSize:0.#} pt, {TextFontStyle}";
        }
    }
}
