using System.Diagnostics;
using System.Drawing;

namespace xNotepad64
{
    public partial class xMain : Form
    {
        private readonly ToolStripStatusLabel _messageStatusLabel = CreateStatusLabel(spring: true);
        private readonly ToolStripStatusLabel _fileSizeStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _chunkStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _offsetStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _chunkModeStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _dirtyStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _fontStatusLabel = CreateStatusLabel();
        private readonly ToolStripStatusLabel _memoryStatusLabel = CreateStatusLabel();
        private readonly System.Windows.Forms.Timer _memoryRefreshTimer = new() { Interval = 2000 };
        private readonly System.Windows.Forms.Timer _undoSnapshotTimer = new() { Interval = 400 };
        private readonly Dictionary<int, ChunkEditHistory> _chunkEditHistories = [];

        private EditorSettings _settings = EditorSettingsStore.Load();
        private searchResults? _searchWindow;
        private Font? _editorFont;
        private bool _suppressSelectionChange;
        private bool _loadingEditorText;
        private bool _currentChunkDirty;
        private bool _isBusy;
        private bool _closeApproved;
        private readonly string? _startupFilePath;
        private string _loadedChunkCommittedText = string.Empty;
        private string _loadedChunkBaselineText = string.Empty;

        private string Program_Version => typeof(xMain).Assembly.GetName().Version?.ToString() ?? "n/a";

        public xMain(string? startupFilePath = null)
        {
            _startupFilePath = startupFilePath;
            InitializeComponent();
            InitializeEditorShell();
            Shown += xMain_Shown;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _settings.MainWindowPlacement = WindowPlacementService.Capture(this);

            if (_searchWindow is not null && !_searchWindow.IsDisposed)
            {
                _settings.SearchWindowPlacement = WindowPlacementService.Capture(_searchWindow);
            }

            PersistSettingsSilently();
            _memoryRefreshTimer.Stop();
            _memoryRefreshTimer.Dispose();
            _undoSnapshotTimer.Stop();
            _undoSnapshotTimer.Dispose();
            _editorFont?.Dispose();
            _editorFont = null;
            base.OnFormClosed(e);
        }

        private void InitializeEditorShell()
        {
            lvwChunks.MultiSelect = false;
            lvwChunks.VirtualMode = true;
            lvwChunks.RetrieveVirtualItem += lvwChunks_RetrieveVirtualItem;

            TextContent.AcceptsTab = true;
            TextContent.WordWrap = false;
            TextContent.MaxLength = int.MaxValue;
            TextContent.HideSelection = false;
            TextContent.TextChanged += TextContent_TextChanged;

            WireDocumentCommands();
            WireEditCommands();
            ApplySettingsToRuntime();
            ApplyStoredWindowPlacement();
            _undoSnapshotTimer.Tick += undoSnapshotTimer_Tick;

            InitializeStatusStrip();
            ApplyLocalization();
            LoadEditorText(string.Empty, string.Empty);
            UpdateWindowState();
        }

        private async void xMain_Shown(object? sender, EventArgs e)
        {
            Shown -= xMain_Shown;

            if (string.IsNullOrWhiteSpace(_startupFilePath))
            {
                return;
            }

            await OpenDocumentAsync(_startupFilePath);
        }

        private void WireDocumentCommands()
        {
            neuToolStripMenuItem.Click += neuToolStripMenuItem_Click;
            neuToolStripButton.Click += neuToolStripMenuItem_Click;
            öffnenToolStripButton.Click += öffnenToolStripMenuItem_Click;
            speichernToolStripMenuItem.Click += speichernToolStripMenuItem_Click;
            speichernToolStripButton.Click += speichernToolStripMenuItem_Click;
            speichernunterToolStripMenuItem.Click += speichernunterToolStripMenuItem_Click;
            beendenToolStripMenuItem.Click += beendenToolStripMenuItem_Click;
            optionenToolStripMenuItem.Click += optionenToolStripMenuItem_Click;
            searchToolStripMenuItem.Click += searchToolStripMenuItem_Click;
            searchToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
            speichernunterToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            FormClosing += xMain_FormClosing;
        }

        private void ApplyLocalization()
        {
            dateiToolStripMenuItem.Text = T("main.menu.file", "&Datei");
            neuToolStripMenuItem.Text = T("main.menu.new", "&Neu");
            öffnenToolStripMenuItem.Text = T("main.menu.open", "Ö&ffnen");
            speichernToolStripMenuItem.Text = T("main.menu.save", "&Speichern");
            speichernunterToolStripMenuItem.Text = T("main.menu.save_as", "Speichern &unter");
            beendenToolStripMenuItem.Text = T("main.menu.exit", "&Beenden");
            bearbeitenToolStripMenuItem.Text = T("main.menu.edit", "&Bearbeiten");
            rückgängigToolStripMenuItem.Text = T("main.menu.undo", "&Rueckgaengig");
            wiederholenToolStripMenuItem.Text = T("main.menu.redo", "&Wiederholen");
            ausschneidenToolStripMenuItem.Text = T("main.menu.cut", "Aussc&hneiden");
            kopierenToolStripMenuItem.Text = T("main.menu.copy", "&Kopieren");
            einfügenToolStripMenuItem.Text = T("main.menu.paste", "&Einfuegen");
            allesauswählenToolStripMenuItem.Text = T("main.menu.select_all", "&Alles auswaehlen");
            searchToolStripMenuItem.Text = T("main.menu.search_replace", "Suchen und ersetzen...");
            extrasToolStripMenuItem.Text = T("main.menu.extras", "E&xtras");
            optionenToolStripMenuItem.Text = T("main.menu.options", "&Optionen");
            hilfeToolStripMenuItem.Text = T("main.menu.help", "&Hilfe");
            infoToolStripMenuItem.Text = T("main.menu.about", "Inf&o...");

            neuToolStripButton.Text = T("main.toolbar.new", "&Neu");
            öffnenToolStripButton.Text = T("main.toolbar.open", "Ö&ffnen");
            speichernToolStripButton.Text = T("main.toolbar.save", "&Speichern");
            ausschneidenToolStripButton.Text = T("main.toolbar.cut", "&Ausschneiden");
            kopierenToolStripButton.Text = T("main.toolbar.copy", "&Kopieren");
            einfügenToolStripButton.Text = T("main.toolbar.paste", "&Einfuegen");

            ChunkCol.Text = T("main.chunk.column", "Block");
            SizeCol.Text = T("main.size.column", "Groesse");

            _searchWindow?.ApplyLocalization();
            UpdateWindowState();
        }

        private void WireEditCommands()
        {
            allesauswählenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;

            rückgängigToolStripMenuItem.Click += rückgängigToolStripMenuItem_Click;
            wiederholenToolStripMenuItem.Click += wiederholenToolStripMenuItem_Click;
            ausschneidenToolStripMenuItem.Click += ausschneidenToolStripMenuItem_Click;
            ausschneidenToolStripButton.Click += ausschneidenToolStripMenuItem_Click;
            kopierenToolStripMenuItem.Click += kopierenToolStripMenuItem_Click;
            kopierenToolStripButton.Click += kopierenToolStripMenuItem_Click;
            einfügenToolStripMenuItem.Click += einfügenToolStripMenuItem_Click;
            einfügenToolStripButton.Click += einfügenToolStripMenuItem_Click;
            allesauswählenToolStripMenuItem.Click += allesauswählenToolStripMenuItem_Click;
        }

        private void lvwChunks_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < 0 || e.ItemIndex >= Textfile.ChunkBlocks.Count)
            {
                e.Item = new ListViewItem(string.Empty);
                return;
            }

            Chunk chunk = Textfile.ChunkBlocks[e.ItemIndex];
            var item = new ListViewItem(BuildChunkLabel(chunk.Index));
            item.SubItems.Add(Textfile.FormatBytes(chunk.Length));
            e.Item = item;
        }

        private async void lvwChunks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChange || _isBusy || lvwChunks.SelectedIndices.Count == 0 || !Textfile.HasOpenFile)
            {
                return;
            }

            int selectedIndex = lvwChunks.SelectedIndices[0];
            if (selectedIndex == Textfile.CurrentChunkIndex && !_loadingEditorText)
            {
                return;
            }

            await SwitchToChunkAsync(selectedIndex, selectionStart: null, selectionLength: 0);
        }

        private void TextContent_TextChanged(object? sender, EventArgs e)
        {
            if (_loadingEditorText || !Textfile.HasOpenFile)
            {
                return;
            }

            _currentChunkDirty = !string.Equals(TextContent.Text, _loadedChunkCommittedText, StringComparison.Ordinal);
            RefreshChunkRow(Textfile.CurrentChunkIndex);
            ScheduleUndoSnapshot();
            UpdateUndoRedoState();
            UpdateStatus();
        }

        private async void neuToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            if (!await ConfirmSaveIfNeededAsync())
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                AddExtension = false,
                FileName = T("main.dialog.new_document_default_name", "Neues Dokument.txt"),
                Filter = T("main.dialog.all_files_filter", "Alle Dateien|*.*"),
                OverwritePrompt = true,
                Title = T("main.dialog.new_document_title", "Neues Dokument erstellen")
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                await CreateNewDocumentAsync(dialog.FileName);
                await OpenDocumentAsync(dialog.FileName);
            }
            catch (Exception ex)
            {
                ShowError(T("main.error.new_document", "Das neue Dokument konnte nicht erstellt werden."), ex);
            }
        }

        private async void öffnenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            if (!await ConfirmSaveIfNeededAsync())
            {
                return;
            }

            using var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = T("main.dialog.all_files_filter", "Alle Dateien|*.*"),
                Title = T("main.dialog.open_title", "Grosse Datei oeffnen")
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            await OpenDocumentAsync(dialog.FileName);
        }

        private async void speichernToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            await SaveDocumentAsync();
        }

        private async void speichernunterToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isBusy || !Textfile.HasOpenFile)
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                AddExtension = false,
                FileName = Textfile.FileName,
                Filter = T("main.dialog.all_files_filter", "Alle Dateien|*.*"),
                InitialDirectory = Path.GetDirectoryName(Textfile.FilePath),
                OverwritePrompt = true,
                Title = T("main.dialog.save_as_title", "Datei speichern unter")
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            await SaveDocumentAsAsync(dialog.FileName);
        }

        private void beendenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void rückgängigToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ExecuteUndo();
        }

        private void wiederholenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ExecuteRedo();
        }

        private void ausschneidenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (!TextContent.ReadOnly)
            {
                TextContent.Focus();
                TextContent.Cut();
            }
        }

        private void kopierenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TextContent.Focus();
            TextContent.Copy();
        }

        private void einfügenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (!TextContent.ReadOnly)
            {
                TextContent.Focus();
                TextContent.Paste();
            }
        }

        private void allesauswählenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TextContent.Focus();
            TextContent.SelectAll();
        }

        private async void optionenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            using var dialog = new Options(_settings);
            WindowPlacementService.Apply(dialog, _settings.OptionsWindowPlacement);

            DialogResult dialogResult = dialog.ShowDialog(this);
            WindowPlacementSettings optionsPlacement = WindowPlacementService.Capture(dialog);
            _settings.OptionsWindowPlacement = optionsPlacement;
            PersistSettingsSilently();

            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            EditorSettings newSettings = dialog.Settings;
            newSettings.OptionsWindowPlacement = optionsPlacement;
            bool chunkSizeChanged = newSettings.MaximumChunkSizeBytes != _settings.MaximumChunkSizeBytes;
            long preferredOffset = GetCurrentAbsoluteOffset();

            if (chunkSizeChanged && Textfile.HasOpenFile && !await ConfirmSaveIfNeededAsync())
            {
                return;
            }

            _settings = newSettings;
            LocalizationDocument? selectedLanguage = LocalizationManager.SetCurrentLanguage(_settings.LanguageFileName);
            if (selectedLanguage is not null)
            {
                _settings.LanguageFileName = selectedLanguage.FileName;
            }

            ApplySettingsToRuntime();
            ApplyLocalization();
            await PersistSettingsAsync();
            UpdateStatus();

            if (chunkSizeChanged && Textfile.HasOpenFile && Textfile.FilePath is not null)
            {
                await OpenDocumentAsync(Textfile.FilePath, preferredOffset);
            }
        }

        private void searchToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_searchWindow is null || _searchWindow.IsDisposed)
            {
                _searchWindow = new searchResults();
                _searchWindow.NavigateToResultRequested += SearchWindow_NavigateToResultRequested;
                _searchWindow.FormClosed += SearchWindow_FormClosed;
                _searchWindow.ConfigureDocumentCallbacks(PrepareSearchWindowDocumentAsync, RefreshVisibleChunkAsync, SetBusyState);
                WindowPlacementService.Apply(_searchWindow, _settings.SearchWindowPlacement);
            }

            _searchWindow.SetSearchTerm(GetSearchSeedText());
            if (!_searchWindow.Visible)
            {
                _searchWindow.Show(this);
            }

            _searchWindow.BringToFront();
            _searchWindow.FocusSearchBox();
        }

        private async void SearchWindow_NavigateToResultRequested(object? sender, SearchResultEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            await SwitchToChunkAsync(e.Result.ChunkIndex, e.Result.PositionInChunk, e.Result.MatchLength);
        }

        private void SearchWindow_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (sender is searchResults window)
            {
                _settings.SearchWindowPlacement = WindowPlacementService.Capture(window);
                PersistSettingsSilently();
            }

            _searchWindow = null;
        }

        private Task PrepareSearchWindowDocumentAsync()
        {
            CommitCurrentChunkIfNeeded();
            return Task.CompletedTask;
        }

        private async Task RefreshVisibleChunkAsync()
        {
            if (!Textfile.HasOpenFile)
            {
                UpdateWindowState();
                return;
            }

            if (Textfile.CurrentChunkIndex < 0 || Textfile.CurrentChunkIndex >= Textfile.ChunkBlocks.Count)
            {
                UpdateWindowState();
                return;
            }

            int selectionStart = TextContent.SelectionStart;
            int selectionLength = TextContent.SelectionLength;
            int currentChunkIndex = Textfile.CurrentChunkIndex;

            string content = await Textfile.GetChunkTextAsync(currentChunkIndex, progress: null, cancellationToken: CancellationToken.None);
            string baselineText = Textfile.TryGetOriginalChunkText(currentChunkIndex, out string originalChunkText)
                ? originalChunkText
                : content;

            ApplyChunkSelection(currentChunkIndex);
            LoadEditorText(content, baselineText);
            ApplySelection(selectionStart, selectionLength);
            UpdateWindowState();
        }

        private async void xMain_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_closeApproved)
            {
                return;
            }

            if (!_currentChunkDirty && !Textfile.HasUnsavedChanges)
            {
                return;
            }

            e.Cancel = true;

            if (await ConfirmSaveIfNeededAsync())
            {
                _closeApproved = true;
                Close();
            }
        }

        private async Task OpenDocumentAsync(string filePath, long? preferredOffset = null)
        {
            try
            {
                ChunkLoadResult result = await RunEditorOperationAsync(
                    T("main.progress.open", "Datei oeffnen"),
                    async (progress, cancellationToken) =>
                    {
                        await Textfile.OpenAsync(filePath, progress, cancellationToken);
                        int initialChunkIndex = preferredOffset.HasValue ? Textfile.FindChunkIndexByOffset(preferredOffset.Value) : 0;
                        initialChunkIndex = Math.Max(initialChunkIndex, 0);
                        string chunkText = Textfile.ChunkBlocks.Count == 0
                            ? string.Empty
                            : await Textfile.GetChunkTextAsync(initialChunkIndex, progress, cancellationToken);

                        return new ChunkLoadResult(initialChunkIndex, chunkText);
                    });

                Textfile.CurrentChunkIndex = Textfile.ChunkBlocks.Count == 0 ? -1 : result.ChunkIndex;
                _currentChunkDirty = false;
                ResetChunkHistories();
                lvwChunks.VirtualListSize = Textfile.ChunkBlocks.Count;
                ApplyChunkSelection(result.ChunkIndex);
                LoadEditorText(result.Text, result.Text);
                _searchWindow?.ResetResults();
                UpdateWindowState();
            }
            catch (OperationCanceledException)
            {
                UpdateStatus(T("main.progress.open_cancelled", "Datei oeffnen abgebrochen."));
            }
            catch (Exception ex)
            {
                ShowError(T("main.error.open_document", "Die Datei konnte nicht geoeffnet werden."), ex);
                UpdateWindowState();
            }
        }

        private async Task<bool> SaveDocumentAsync()
        {
            if (!Textfile.HasOpenFile)
            {
                return true;
            }

            CommitCurrentChunkIfNeeded();

            if (!Textfile.HasUnsavedChanges)
            {
                UpdateStatus();
                return true;
            }

            long preferredOffset = GetCurrentAbsoluteOffset();

            try
            {
                ChunkLoadResult result = await RunEditorOperationAsync(
                    T("main.progress.save", "Datei speichern"),
                    async (progress, cancellationToken) =>
                    {
                        await Textfile.SaveAsync(progress, cancellationToken);
                        int chunkIndex = Textfile.ChunkBlocks.Count == 0 ? -1 : Textfile.FindChunkIndexByOffset(preferredOffset);
                        string chunkText = chunkIndex >= 0
                            ? await Textfile.GetChunkTextAsync(chunkIndex, progress, cancellationToken)
                            : string.Empty;

                        return new ChunkLoadResult(chunkIndex, chunkText);
                    });

                Textfile.CurrentChunkIndex = result.ChunkIndex;
                ResetChunkHistories();
                lvwChunks.VirtualListSize = Textfile.ChunkBlocks.Count;
                ApplyChunkSelection(result.ChunkIndex);
                LoadEditorText(result.Text, result.Text);
                UpdateWindowState();
                return true;
            }
            catch (OperationCanceledException)
            {
                UpdateStatus(T("main.progress.save_cancelled", "Speichern abgebrochen."));
                return false;
            }
            catch (Exception ex)
            {
                ShowError(T("main.error.save_document", "Die Datei konnte nicht gespeichert werden."), ex);
                return false;
            }
        }

        private async Task<bool> SaveDocumentAsAsync(string filePath)
        {
            if (!Textfile.HasOpenFile)
            {
                return false;
            }

            CommitCurrentChunkIfNeeded();
            long preferredOffset = GetCurrentAbsoluteOffset();

            try
            {
                ChunkLoadResult result = await RunEditorOperationAsync(
                    T("main.progress.save_as", "Datei speichern unter"),
                    async (progress, cancellationToken) =>
                    {
                        await Textfile.SaveAsAsync(filePath, progress, cancellationToken);
                        int chunkIndex = Textfile.ChunkBlocks.Count == 0 ? -1 : Textfile.FindChunkIndexByOffset(preferredOffset);
                        string chunkText = chunkIndex >= 0
                            ? await Textfile.GetChunkTextAsync(chunkIndex, progress, cancellationToken)
                            : string.Empty;

                        return new ChunkLoadResult(chunkIndex, chunkText);
                    });

                Textfile.CurrentChunkIndex = result.ChunkIndex;
                ResetChunkHistories();
                lvwChunks.VirtualListSize = Textfile.ChunkBlocks.Count;
                ApplyChunkSelection(result.ChunkIndex);
                LoadEditorText(result.Text, result.Text);
                _searchWindow?.ResetResults();
                UpdateWindowState();
                return true;
            }
            catch (OperationCanceledException)
            {
                UpdateStatus(T("main.progress.save_as_cancelled", "Speichern unter abgebrochen."));
                return false;
            }
            catch (Exception ex)
            {
                ShowError(T("main.error.save_as_document", "Die Datei konnte nicht unter dem neuen Namen gespeichert werden."), ex);
                return false;
            }
        }

        private async Task SwitchToChunkAsync(int chunkIndex, int? selectionStart, int selectionLength)
        {
            try
            {
                CommitCurrentChunkIfNeeded();

                ChunkLoadResult result = await RunEditorOperationAsync(
                    T("main.progress.load_chunk", "Chunk laden"),
                    async (progress, cancellationToken) =>
                    {
                        string text = await Textfile.GetChunkTextAsync(chunkIndex, progress, cancellationToken);
                        return new ChunkLoadResult(chunkIndex, text);
                    });

                Textfile.CurrentChunkIndex = result.ChunkIndex;
                string baselineText = Textfile.TryGetOriginalChunkText(result.ChunkIndex, out string originalChunkText)
                    ? originalChunkText
                    : result.Text;

                ApplyChunkSelection(result.ChunkIndex);
                LoadEditorText(result.Text, baselineText);
                ApplySelection(selectionStart, selectionLength);
                UpdateStatus();
            }
            catch (OperationCanceledException)
            {
                RestoreCurrentSelection();
                UpdateStatus(T("main.progress.load_chunk_cancelled", "Chunk-Ladevorgang abgebrochen."));
            }
            catch (Exception ex)
            {
                RestoreCurrentSelection();
                ShowError(T("main.error.load_chunk", "Der ausgewaehlte Chunk konnte nicht geladen werden."), ex);
            }
        }

        private void CommitCurrentChunkIfNeeded()
        {
            if (!_currentChunkDirty || !Textfile.HasOpenFile || Textfile.CurrentChunkIndex < 0)
            {
                return;
            }

            CommitPendingUndoSnapshotIfNeeded();
            Textfile.SetChunkDraft(Textfile.CurrentChunkIndex, TextContent.Text, _loadedChunkBaselineText);
            _loadedChunkCommittedText = TextContent.Text;
            _currentChunkDirty = false;
            RefreshChunkRow(Textfile.CurrentChunkIndex);
            EnsureCurrentChunkHistory().ReplaceCurrentSelection(TextContent.SelectionStart, TextContent.SelectionLength);
            UpdateUndoRedoState();
            UpdateStatus();
        }

        private void LoadEditorText(string content, string baselineText)
        {
            _undoSnapshotTimer.Stop();
            _loadingEditorText = true;

            try
            {
                TextContent.Text = content;
                TextContent.SelectionStart = 0;
                TextContent.SelectionLength = 0;
                _loadedChunkCommittedText = content;
                _loadedChunkBaselineText = baselineText;
                _currentChunkDirty = false;
            }
            finally
            {
                _loadingEditorText = false;
            }

            AttachCurrentChunkHistory(content);
            UpdateUndoRedoState();
        }

        private void ApplyChunkSelection(int chunkIndex)
        {
            _suppressSelectionChange = true;

            try
            {
                lvwChunks.SelectedIndices.Clear();

                if (chunkIndex >= 0 && chunkIndex < lvwChunks.VirtualListSize)
                {
                    lvwChunks.SelectedIndices.Add(chunkIndex);
                    lvwChunks.EnsureVisible(chunkIndex);
                }
            }
            finally
            {
                _suppressSelectionChange = false;
            }
        }

        private void ApplySelection(int? selectionStart, int selectionLength)
        {
            if (!selectionStart.HasValue || selectionStart.Value < 0)
            {
                TextContent.SelectionStart = 0;
                TextContent.SelectionLength = 0;
                return;
            }

            int boundedStart = Math.Min(selectionStart.Value, TextContent.TextLength);
            int boundedLength = Math.Max(0, Math.Min(selectionLength, TextContent.TextLength - boundedStart));

            TextContent.SelectionStart = boundedStart;
            TextContent.SelectionLength = boundedLength;
            TextContent.ScrollToCaret();
            TextContent.Focus();
        }

        private void ExecuteUndo()
        {
            if (!CanEditCurrentChunk())
            {
                return;
            }

            CommitPendingUndoSnapshotIfNeeded();
            ChunkEditHistory history = EnsureCurrentChunkHistory();
            if (!history.CanUndo)
            {
                return;
            }

            ApplyHistoryState(history.Undo());
        }

        private void ExecuteRedo()
        {
            if (!CanEditCurrentChunk())
            {
                return;
            }

            CommitPendingUndoSnapshotIfNeeded();
            ChunkEditHistory history = EnsureCurrentChunkHistory();
            if (!history.CanRedo)
            {
                return;
            }

            ApplyHistoryState(history.Redo());
        }

        private bool CanEditCurrentChunk()
        {
            return Textfile.HasOpenFile && Textfile.CurrentChunkIndex >= 0 && !_isBusy;
        }

        private void ScheduleUndoSnapshot()
        {
            if (!CanEditCurrentChunk())
            {
                return;
            }

            _undoSnapshotTimer.Stop();
            _undoSnapshotTimer.Start();
        }

        private void undoSnapshotTimer_Tick(object? sender, EventArgs e)
        {
            _undoSnapshotTimer.Stop();
            CommitPendingUndoSnapshotIfNeeded();
        }

        private void CommitPendingUndoSnapshotIfNeeded()
        {
            _undoSnapshotTimer.Stop();

            if (!CanEditCurrentChunk())
            {
                return;
            }

            EnsureCurrentChunkHistory().RecordState(TextContent.Text, TextContent.SelectionStart, TextContent.SelectionLength);
            UpdateUndoRedoState();
        }

        private ChunkEditHistory EnsureCurrentChunkHistory()
        {
            if (!Textfile.HasOpenFile || Textfile.CurrentChunkIndex < 0)
            {
                throw new InvalidOperationException(T("main.error.no_editable_chunk", "Es ist aktuell kein bearbeitbarer Chunk aktiv."));
            }

            if (!_chunkEditHistories.TryGetValue(Textfile.CurrentChunkIndex, out ChunkEditHistory? history))
            {
                history = new ChunkEditHistory(TextContent.Text, TextContent.SelectionStart, TextContent.SelectionLength);
                _chunkEditHistories[Textfile.CurrentChunkIndex] = history;
            }

            return history;
        }

        private void AttachCurrentChunkHistory(string content)
        {
            _undoSnapshotTimer.Stop();

            if (!Textfile.HasOpenFile || Textfile.CurrentChunkIndex < 0)
            {
                return;
            }

            if (_chunkEditHistories.TryGetValue(Textfile.CurrentChunkIndex, out ChunkEditHistory? history) &&
                string.Equals(history.CurrentState.Text, content, StringComparison.Ordinal))
            {
                history.ReplaceCurrentSelection(TextContent.SelectionStart, TextContent.SelectionLength);
                return;
            }

            _chunkEditHistories[Textfile.CurrentChunkIndex] = new ChunkEditHistory(content, TextContent.SelectionStart, TextContent.SelectionLength);
        }

        private void ResetChunkHistories()
        {
            _undoSnapshotTimer.Stop();
            _chunkEditHistories.Clear();
            UpdateUndoRedoState();
        }

        private void UpdateUndoRedoState()
        {
            bool hasEditableChunk = CanEditCurrentChunk();
            ChunkEditHistory? history = null;
            bool hasPendingTextDifference = false;

            if (hasEditableChunk)
            {
                _chunkEditHistories.TryGetValue(Textfile.CurrentChunkIndex, out history);
                hasPendingTextDifference = history is not null &&
                    !string.Equals(TextContent.Text, history.CurrentState.Text, StringComparison.Ordinal);
            }

            rückgängigToolStripMenuItem.Enabled = hasEditableChunk && history is not null && (history.CanUndo || hasPendingTextDifference);
            wiederholenToolStripMenuItem.Enabled = hasEditableChunk && history is not null && history.CanRedo && !hasPendingTextDifference;
        }

        private void ApplyHistoryState(ChunkTextState state)
        {
            _undoSnapshotTimer.Stop();
            _loadingEditorText = true;

            try
            {
                TextContent.Text = state.Text;
                TextContent.SelectionStart = Math.Min(state.SelectionStart, TextContent.TextLength);
                TextContent.SelectionLength = Math.Max(0, Math.Min(state.SelectionLength, TextContent.TextLength - TextContent.SelectionStart));
            }
            finally
            {
                _loadingEditorText = false;
            }

            _currentChunkDirty = !string.Equals(TextContent.Text, _loadedChunkCommittedText, StringComparison.Ordinal);
            RefreshChunkRow(Textfile.CurrentChunkIndex);
            TextContent.ScrollToCaret();
            TextContent.Focus();
            UpdateUndoRedoState();
            UpdateStatus();
        }

        private async Task<bool> ConfirmSaveIfNeededAsync()
        {
            if (!_currentChunkDirty && !Textfile.HasUnsavedChanges)
            {
                return true;
            }

            DialogResult result = MessageBox.Show(
                T("main.dialog.unsaved_message", "Es gibt ungespeicherte Aenderungen in den geladenen Chunks. Soll die Datei vor dem Fortfahren gespeichert werden?"),
                T("main.dialog.unsaved_title", "Ungespeicherte Aenderungen"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            return result switch
            {
                DialogResult.Yes => await SaveDocumentAsync(),
                DialogResult.No => true,
                _ => false
            };
        }

        private async Task<T> RunEditorOperationAsync<T>(string title, Func<IProgress<OperationProgress>, CancellationToken, Task<T>> operation)
        {
            SetBusyState(true);

            try
            {
                return await ProgressDialogRunner.RunAsync(this, title, operation);
            }
            finally
            {
                SetBusyState(false);
            }
        }

        private void SetBusyState(bool isBusy)
        {
            _isBusy = isBusy;
            menuStrip1.Enabled = !isBusy;
            toolStrip1.Enabled = !isBusy;
            splitContainer1.Enabled = !isBusy;
            UseWaitCursor = isBusy;
            _searchWindow?.SetDocumentBusy(isBusy);
            UpdateUndoRedoState();
        }

        private void InitializeStatusStrip()
        {
            statusStrip1.SizingGrip = false;
            statusStrip1.Items.Clear();
            statusStrip1.Items.AddRange(
            [
                _messageStatusLabel,
                CreateStatusSeparator(),
                _fileSizeStatusLabel,
                CreateStatusSeparator(),
                _chunkStatusLabel,
                CreateStatusSeparator(),
                _offsetStatusLabel,
                CreateStatusSeparator(),
                _chunkModeStatusLabel,
                CreateStatusSeparator(),
                _dirtyStatusLabel,
                CreateStatusSeparator(),
                _fontStatusLabel,
                CreateStatusSeparator(),
                _memoryStatusLabel
            ]);

            _memoryRefreshTimer.Tick += memoryRefreshTimer_Tick;
            _memoryRefreshTimer.Start();
            RefreshMemoryStatus();
        }

        private void ApplyStoredWindowPlacement()
        {
            WindowPlacementService.Apply(this, _settings.MainWindowPlacement);
        }

        private void ApplySettingsToRuntime()
        {
            _settings.Normalize();
            Textfile.MaximumChunkSize = _settings.MaximumChunkSizeBytes;
            Textfile.AllowAbruptChunkCutoff = _settings.AllowAbruptChunkCutoff;
            ApplyEditorFont();
        }

        private async Task PersistSettingsAsync()
        {
            _settings.Normalize();
            await EditorSettingsStore.SaveAsync(_settings);
        }

        private void PersistSettingsSilently()
        {
            try
            {
                _settings.Normalize();
                EditorSettingsStore.Save(_settings);
            }
            catch
            {
                // Fensterpositionen sollen das Beenden nicht blockieren.
            }
        }

        private void ApplyEditorFont()
        {
            Font newFont = _settings.CreateTextFont();
            Font? previousFont = _editorFont;
            _editorFont = newFont;
            TextContent.Font = _editorFont;
            previousFont?.Dispose();
        }

        private void UpdateWindowState()
        {
            bool hasOpenFile = Textfile.HasOpenFile;

            Text = hasOpenFile
                ? TF("main.title.with_file", "{0} - {1} [{2}]", Textfile.FileName, T("app.name", "xNotepad64"), Program_Version)
                : TF("main.title.no_file", "{0} [{1}]", T("app.name", "xNotepad64"), Program_Version);
            TextContent.ReadOnly = !hasOpenFile;
            lvwChunks.Enabled = hasOpenFile && !_isBusy;
            speichernToolStripMenuItem.Enabled = hasOpenFile;
            speichernunterToolStripMenuItem.Enabled = hasOpenFile;
            speichernToolStripButton.Enabled = hasOpenFile;

            if (!hasOpenFile)
            {
                lvwChunks.VirtualListSize = 0;
                LoadEditorText(string.Empty, string.Empty);
            }

            UpdateUndoRedoState();
            UpdateStatus();
        }

        private void UpdateStatus(string? overrideMessage = null)
        {
            int dirtyChunkCount = GetDirtyChunkCount();
            _messageStatusLabel.Text = !string.IsNullOrWhiteSpace(overrideMessage)
                ? overrideMessage
                : BuildDefaultStatusMessage(dirtyChunkCount);
            _messageStatusLabel.ToolTipText = Textfile.FilePath ?? T("main.status.no_file_tooltip", "Keine Datei geoeffnet.");

            UpdateFileSizeStatus();
            UpdateChunkStatus();
            UpdateOffsetStatus();
            UpdateChunkModeStatus();
            UpdateDirtyStatus(dirtyChunkCount);
            UpdateFontStatus();
            RefreshMemoryStatus();
        }

        private int GetDirtyChunkCount()
        {
            int dirtyChunkCount = Textfile.ModifiedChunkCount;

            if (Textfile.HasOpenFile &&
                _currentChunkDirty &&
                Textfile.CurrentChunkIndex >= 0 &&
                !Textfile.IsChunkModified(Textfile.CurrentChunkIndex))
            {
                dirtyChunkCount++;
            }

            return dirtyChunkCount;
        }

        private string BuildDefaultStatusMessage(int dirtyChunkCount)
        {
            if (!Textfile.HasOpenFile)
            {
                return T("main.status.no_file", "Keine Datei geoeffnet.");
            }

            return dirtyChunkCount > 0
                ? T("main.status.changes_in_memory", "Aenderungen im Speicher.")
                : T("main.status.ready", "Bereit.");
        }

        private void UpdateFileSizeStatus()
        {
            if (!Textfile.HasOpenFile)
            {
                _fileSizeStatusLabel.Text = $"{T("main.status.file_prefix", "Datei")} -";
                _fileSizeStatusLabel.ToolTipText = T("main.status.no_file_tooltip", "Keine Datei geoeffnet.");
                return;
            }

            long currentLength = Textfile.CurrentDocumentLength;
            long savedLength = Textfile.SavedFileLength;
            bool hasProjectedSize = currentLength != savedLength;

            _fileSizeStatusLabel.Text = $"{T("main.status.file_prefix", "Datei")} {Textfile.FormatBytes(currentLength)}{(hasProjectedSize ? "*" : string.Empty)}";
            _fileSizeStatusLabel.ToolTipText = hasProjectedSize
                ? TF("main.status.file_size_dirty_tooltip", "Aktuelle Dokumentgroesse im Speicher: {0}. Zuletzt gespeichert: {1}.", Textfile.FormatBytes(currentLength), Textfile.FormatBytes(savedLength))
                : TF("main.status.file_size_tooltip", "Aktuelle Dokumentgroesse: {0}.", Textfile.FormatBytes(currentLength));
        }

        private void UpdateChunkStatus()
        {
            if (!Textfile.HasOpenFile)
            {
                _chunkStatusLabel.Text = $"{T("main.status.chunks_prefix", "Chunks")} -";
                _chunkStatusLabel.ToolTipText = T("main.status.no_file_tooltip", "Keine Datei geoeffnet.");
                return;
            }

            if (Textfile.CurrentChunkIndex < 0 || Textfile.CurrentChunkIndex >= Textfile.ChunkBlocks.Count)
            {
                _chunkStatusLabel.Text = $"{Textfile.ChunkBlocks.Count:N0} {T("main.status.chunks_prefix", "Chunks")}";
                _chunkStatusLabel.ToolTipText = T("main.status.no_active_chunk_tooltip", "Es ist aktuell kein Chunk aktiv.");
                return;
            }

            Chunk currentChunk = Textfile.ChunkBlocks[Textfile.CurrentChunkIndex];
            _chunkStatusLabel.Text = $"{T("main.status.chunk_prefix", "Chunk")} {Textfile.CurrentChunkIndex + 1}/{Textfile.ChunkBlocks.Count} ({Textfile.FormatBytes(currentChunk.Length)})";
            _chunkStatusLabel.ToolTipText = TF("main.status.chunk_tooltip", "Aktueller Chunk startet bei Byte {0} und endet vor Byte {1}.", Textfile.FormatOffset(currentChunk.Start), Textfile.FormatOffset(currentChunk.EndExclusive));
        }

        private void UpdateOffsetStatus()
        {
            if (!Textfile.HasOpenFile || Textfile.CurrentChunkIndex < 0 || Textfile.CurrentChunkIndex >= Textfile.ChunkBlocks.Count)
            {
                _offsetStatusLabel.Text = $"{T("main.status.offset_prefix", "Offset")} -";
                _offsetStatusLabel.ToolTipText = T("main.status.no_active_offset_tooltip", "Kein aktiver Chunk.");
                return;
            }

            Chunk currentChunk = Textfile.ChunkBlocks[Textfile.CurrentChunkIndex];
            _offsetStatusLabel.Text = $"{T("main.status.offset_prefix", "Offset")} {Textfile.FormatOffset(currentChunk.Start)}";
            _offsetStatusLabel.ToolTipText = TF("main.status.offset_tooltip", "Absoluter Startoffset des aktiven Chunks: {0}.", Textfile.FormatOffset(currentChunk.Start));
        }

        private void UpdateChunkModeStatus()
        {
            string cutMode = _settings.AllowAbruptChunkCutoff
                ? T("main.status.mode.abrupt", "abrupt")
                : T("main.status.mode.word_safe", "wortschonend");
            _chunkModeStatusLabel.Text = $"{T("main.status.limit_prefix", "Limit")} {Textfile.FormatBytes(_settings.MaximumChunkSizeBytes)}, {cutMode}";
            _chunkModeStatusLabel.ToolTipText = TF("main.status.mode_tooltip", "Maximale Chunk-Groesse: {0}. Schnittmodus: {1}.", Textfile.FormatBytes(_settings.MaximumChunkSizeBytes), cutMode);
        }

        private void UpdateDirtyStatus(int dirtyChunkCount)
        {
            _dirtyStatusLabel.Text = $"{T("main.status.edits_prefix", "Aend.")} {dirtyChunkCount:N0}";
            _dirtyStatusLabel.ToolTipText = dirtyChunkCount == 0
                ? T("main.status.no_edits_tooltip", "Keine ungespeicherten Chunk-Aenderungen.")
                : TF("main.status.edits_tooltip", "{0} Chunk(s) enthalten ungespeicherte Aenderungen.", dirtyChunkCount);
        }

        private void UpdateFontStatus()
        {
            _fontStatusLabel.Text = $"{T("main.status.font_prefix", "Font")} {_settings.TextFontFamily}, {_settings.TextFontSize:0.#} pt";
            _fontStatusLabel.ToolTipText = _settings.GetTextFontSummary();
        }

        private void RefreshMemoryStatus()
        {
            if (!SystemMemoryInfo.TryGetSnapshot(out SystemMemorySnapshot snapshot))
            {
                _memoryStatusLabel.Text = T("main.status.memory_na", "RAM n/a");
                _memoryStatusLabel.ToolTipText = T("main.status.memory_unavailable", "RAM-Information konnte nicht gelesen werden.");
                return;
            }

            using Process process = Process.GetCurrentProcess();
            process.Refresh();

            long workingSetBytes = process.WorkingSet64;
            long availableBytes = ClampToInt64(snapshot.AvailablePhysicalBytes);
            long totalBytes = ClampToInt64(snapshot.TotalPhysicalBytes);

            _memoryStatusLabel.Text = TF("main.status.memory_text", "{0} {1} von {2} verf.", T("main.status.memory_prefix", "RAM"), Textfile.FormatBytes(workingSetBytes), Textfile.FormatBytes(availableBytes));
            _memoryStatusLabel.ToolTipText = TF("main.status.memory_tooltip", "Working Set der App: {0}. Verfuegbarer physischer RAM: {1} von {2} gesamt.", Textfile.FormatBytes(workingSetBytes), Textfile.FormatBytes(availableBytes), Textfile.FormatBytes(totalBytes));
        }

        private void memoryRefreshTimer_Tick(object? sender, EventArgs e)
        {
            RefreshMemoryStatus();
        }

        private string BuildChunkLabel(int chunkIndex)
        {
            string prefix = IsChunkDirty(chunkIndex) ? "* " : string.Empty;
            return $"{prefix}{T("main.status.chunk_prefix", "Chunk")} {chunkIndex + 1}";
        }

        private bool IsChunkDirty(int chunkIndex)
        {
            return Textfile.IsChunkModified(chunkIndex) || (_currentChunkDirty && chunkIndex == Textfile.CurrentChunkIndex);
        }

        private void RefreshChunkRow(int chunkIndex)
        {
            if (chunkIndex < 0 || chunkIndex >= lvwChunks.VirtualListSize)
            {
                return;
            }

            lvwChunks.RedrawItems(chunkIndex, chunkIndex, invalidateOnly: true);
        }

        private void RestoreCurrentSelection()
        {
            if (Textfile.CurrentChunkIndex < 0 || Textfile.CurrentChunkIndex >= lvwChunks.VirtualListSize)
            {
                return;
            }

            ApplyChunkSelection(Textfile.CurrentChunkIndex);
        }

        private long GetCurrentAbsoluteOffset()
        {
            if (!Textfile.HasOpenFile || Textfile.CurrentChunkIndex < 0 || Textfile.CurrentChunkIndex >= Textfile.ChunkBlocks.Count)
            {
                return 0;
            }

            Chunk currentChunk = Textfile.ChunkBlocks[Textfile.CurrentChunkIndex];
            return currentChunk.Start + TextContent.SelectionStart;
        }

        private string GetSearchSeedText()
        {
            return string.IsNullOrWhiteSpace(TextContent.SelectedText) ? string.Empty : TextContent.SelectedText;
        }

        private static ToolStripStatusLabel CreateStatusLabel(bool spring = false)
        {
            return new ToolStripStatusLabel
            {
                Spring = spring,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static ToolStripSeparator CreateStatusSeparator()
        {
            return new ToolStripSeparator();
        }

        private static long ClampToInt64(ulong value)
        {
            return value > long.MaxValue ? long.MaxValue : (long)value;
        }

        private static async Task CreateNewDocumentAsync(string filePath)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 1, options: FileOptions.Asynchronous);
            await stream.FlushAsync();
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}{Environment.NewLine}{Environment.NewLine}{ex.Message}", T("app.name", "xNotepad64"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string T(string key, string fallback)
        {
            return LocalizationManager.Get(key, fallback);
        }

        private static string TF(string key, string fallback, params object[] arguments)
        {
            return LocalizationManager.Format(key, fallback, arguments);
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var abdlg = new aboutWindow();
            abdlg.ShowDialog(this);
        }

        private sealed record ChunkLoadResult(int ChunkIndex, string Text);
    }
}
