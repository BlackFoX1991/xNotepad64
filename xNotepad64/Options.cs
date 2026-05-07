namespace xNotepad64
{
    public partial class Options : Form
    {
        private Font? _previewFont;

        public Options(EditorSettings currentSettings)
        {
            InitializeComponent();
            Settings = currentSettings.Clone();
            InitializeOptionsLogic();
        }

        public EditorSettings Settings { get; }

        private void InitializeOptionsLogic()
        {
            Settings.Normalize();

            chunkSizeNumericUpDown.Minimum = EditorSettings.MinimumChunkSizeBytes / EditorSettings.BytesPerMiB;
            chunkSizeNumericUpDown.Maximum = EditorSettings.MaximumChunkSizeBytesLimit / EditorSettings.BytesPerMiB;
            chunkSizeNumericUpDown.Increment = 1;

            chunkSizeNumericUpDown.Value = Math.Clamp(
                Settings.MaximumChunkSizeBytes / EditorSettings.BytesPerMiB,
                (long)chunkSizeNumericUpDown.Minimum,
                (long)chunkSizeNumericUpDown.Maximum);
            allowAbruptChunkCutoffCheckBox.Checked = Settings.AllowAbruptChunkCutoff;

            chunkSizeNumericUpDown.ValueChanged += chunkSizeNumericUpDown_ValueChanged;
            allowAbruptChunkCutoffCheckBox.CheckedChanged += allowAbruptChunkCutoffCheckBox_CheckedChanged;
            chooseFontButton.Click += chooseFontButton_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            FormClosed += Options_FormClosed;

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            UpdateSizeInfo();
            UpdateFontInfo();
        }

        private void chunkSizeNumericUpDown_ValueChanged(object? sender, EventArgs e)
        {
            UpdateSizeInfo();
        }

        private void allowAbruptChunkCutoffCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateSizeInfo();
        }

        private void chooseFontButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new FontDialog
            {
                Font = Settings.CreateTextFont(),
                FontMustExist = true,
                ShowApply = false,
                ShowColor = false,
                ShowEffects = false
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Settings.TextFontFamily = dialog.Font.FontFamily.Name;
            Settings.TextFontSize = dialog.Font.Size;
            Settings.TextFontStyle = dialog.Font.Style;
            Settings.Normalize();
            UpdateFontInfo();
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            Settings.MaximumChunkSizeBytes = (long)chunkSizeNumericUpDown.Value * EditorSettings.BytesPerMiB;
            Settings.AllowAbruptChunkCutoff = allowAbruptChunkCutoffCheckBox.Checked;
            Settings.Normalize();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Options_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _previewFont?.Dispose();
            _previewFont = null;
        }

        private void UpdateSizeInfo()
        {
            long chunkBytes = (long)chunkSizeNumericUpDown.Value * EditorSettings.BytesPerMiB;
            double chunkMiB = chunkBytes / (double)EditorSettings.BytesPerMiB;
            double chunkGiB = chunkBytes / (double)EditorSettings.BytesPerGiB;

            bytesValueLabel.Text = $"{chunkBytes:N0} Bytes";
            mibValueLabel.Text = $"{chunkMiB:N0} MiB";
            gibValueLabel.Text = $"{chunkGiB:0.###} GiB";
            hintLabel.Text = allowAbruptChunkCutoffCheckBox.Checked
                ? "Abruptes Abschneiden ist aktiv. Chunks enden direkt am Limit, nur Zeichenkodierung bleibt gueltig."
                : "Wortschutz ist aktiv. Chunks duerfen ueber das Limit hinauswachsen, damit Woerter und Zahlenfolgen nicht mitten drin getrennt werden.";
        }

        private void UpdateFontInfo()
        {
            fontNameValueLabel.Text = Settings.TextFontFamily;
            fontSizeValueLabel.Text = $"{Settings.TextFontSize:0.#} pt";
            fontStyleValueLabel.Text = Settings.TextFontStyle.ToString();
            fontSummaryLabel.Text = Settings.GetTextFontSummary();

            Font previewFont = Settings.CreateTextFont();
            fontPreviewLabel.Font = previewFont;
            fontPreviewLabel.Text = "Beispiel: 0123456789 ABC abc []{}()";

            _previewFont?.Dispose();
            _previewFont = previewFont;
        }
    }
}
