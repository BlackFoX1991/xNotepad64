namespace xNotepad64
{
    public partial class Options : Form
    {
        private Font? _previewFont;
        private IReadOnlyList<LocalizationDocument> _availableLanguages = [];

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
            _availableLanguages = LocalizationManager.ReloadAvailableLanguages();
            InitializeLanguageSelection();

            chunkSizeNumericUpDown.ValueChanged += chunkSizeNumericUpDown_ValueChanged;
            allowAbruptChunkCutoffCheckBox.CheckedChanged += allowAbruptChunkCutoffCheckBox_CheckedChanged;
            languageComboBox.SelectedIndexChanged += languageComboBox_SelectedIndexChanged;
            chooseFontButton.Click += chooseFontButton_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            FormClosed += Options_FormClosed;

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            ApplyLocalization();
            UpdateSizeInfo();
            UpdateFontInfo();
            UpdateLanguageInfo();
        }

        public void ApplyLocalization()
        {
            Text = LocalizationManager.Get("options.form.title", "Optionen");
            chunkSizeLabel.Text = LocalizationManager.Get("options.chunk_size_label", "Maximale Chunk-Groesse (MiB)");
            allowAbruptChunkCutoffCheckBox.Text = LocalizationManager.Get("options.allow_abrupt", "Konsequentes Abschneiden von Woertern und Zahlen erlauben");
            infoGroupBox.Text = LocalizationManager.Get("options.info_group", "Aktuelle Chunk-Groesse");
            bytesCaptionLabel.Text = LocalizationManager.Get("options.bytes", "Bytes:");
            mibCaptionLabel.Text = LocalizationManager.Get("options.mib", "MiB (binaer):");
            gibCaptionLabel.Text = LocalizationManager.Get("options.gib", "GiB (binaer):");
            languageGroupBox.Text = LocalizationManager.Get("options.language_group", "Sprache");
            languageLabel.Text = LocalizationManager.Get("options.language_label", "UI-Sprache:");
            languageCultureCaptionLabel.Text = LocalizationManager.Get("options.language_culture", "Kultur:");
            languageVersionCaptionLabel.Text = LocalizationManager.Get("options.language_version", "Version:");
            languageAuthorCaptionLabel.Text = LocalizationManager.Get("options.language_author", "Autor:");
            languageDescriptionCaptionLabel.Text = LocalizationManager.Get("options.language_description", "Beschreibung:");
            fontGroupBox.Text = LocalizationManager.Get("options.font_group", "Editor-Schrift");
            fontNameCaptionLabel.Text = LocalizationManager.Get("options.font_name", "Font:");
            fontSizeCaptionLabel.Text = LocalizationManager.Get("options.font_size", "Groesse:");
            fontStyleCaptionLabel.Text = LocalizationManager.Get("options.font_style", "Stil:");
            chooseFontButton.Text = LocalizationManager.Get("options.font_choose", "Schrift waehlen");
            btnSave.Text = LocalizationManager.Get("options.save", "Speichern");
            btnCancel.Text = LocalizationManager.Get("options.cancel", "Abbrechen");
        }

        private void chunkSizeNumericUpDown_ValueChanged(object? sender, EventArgs e)
        {
            UpdateSizeInfo();
        }

        private void allowAbruptChunkCutoffCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateSizeInfo();
        }

        private void languageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateLanguageInfo();
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
            Settings.LanguageFileName = GetSelectedLanguage()?.FileName ?? Settings.LanguageFileName;
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
                ? LocalizationManager.Get("options.hint.abrupt", "Abruptes Abschneiden ist aktiv. Chunks enden direkt am Limit, nur die Zeichenkodierung bleibt gueltig.")
                : LocalizationManager.Get("options.hint.word_safe", "Wortschutz ist aktiv. Chunks duerfen ueber das Limit hinauswachsen, damit Woerter und Zahlenfolgen nicht mitten drin getrennt werden.");
        }

        private void UpdateFontInfo()
        {
            fontNameValueLabel.Text = Settings.TextFontFamily;
            fontSizeValueLabel.Text = $"{Settings.TextFontSize:0.#} pt";
            fontStyleValueLabel.Text = Settings.TextFontStyle.ToString();
            fontSummaryLabel.Text = Settings.GetTextFontSummary();

            Font previewFont = Settings.CreateTextFont();
            fontPreviewLabel.Font = previewFont;
            fontPreviewLabel.Text = LocalizationManager.Get("options.font_preview", "Beispiel: 0123456789 ABC abc []{}()");

            _previewFont?.Dispose();
            _previewFont = previewFont;
        }

        private void InitializeLanguageSelection()
        {
            languageComboBox.DisplayMember = nameof(LocalizationDocument.Title);
            languageComboBox.ValueMember = nameof(LocalizationDocument.FileName);
            languageComboBox.Items.Clear();

            foreach (LocalizationDocument language in _availableLanguages)
            {
                languageComboBox.Items.Add(language);
            }

            LocalizationDocument? selectedLanguage = _availableLanguages.FirstOrDefault(language =>
                string.Equals(language.FileName, Settings.LanguageFileName, StringComparison.OrdinalIgnoreCase))
                ?? _availableLanguages.FirstOrDefault();

            if (selectedLanguage is not null)
            {
                languageComboBox.SelectedItem = selectedLanguage;
            }

            languageComboBox.Enabled = _availableLanguages.Count > 0;
        }

        private void UpdateLanguageInfo()
        {
            LocalizationDocument? selectedLanguage = GetSelectedLanguage();
            if (selectedLanguage is null)
            {
                string noLanguagesText = LocalizationManager.Get("options.language_none", "Keine Sprachdateien gefunden.");
                languageCultureValueLabel.Text = "-";
                languageVersionValueLabel.Text = "-";
                languageAuthorValueLabel.Text = "-";
                languageDescriptionValueLabel.Text = noLanguagesText;
                return;
            }

            languageCultureValueLabel.Text = string.IsNullOrWhiteSpace(selectedLanguage.CultureName) ? "-" : selectedLanguage.CultureName;
            languageVersionValueLabel.Text = string.IsNullOrWhiteSpace(selectedLanguage.Version) ? "-" : selectedLanguage.Version;
            languageAuthorValueLabel.Text = string.IsNullOrWhiteSpace(selectedLanguage.Author) ? "-" : selectedLanguage.Author;
            languageDescriptionValueLabel.Text = string.IsNullOrWhiteSpace(selectedLanguage.Description) ? "-" : selectedLanguage.Description;
        }

        private LocalizationDocument? GetSelectedLanguage()
        {
            return languageComboBox.SelectedItem as LocalizationDocument;
        }
    }
}
