namespace xNotepad64
{
    public partial class searchResults : Form
    {
        private List<SearchResult> _searchResults = [];
        private string? _lastSearchSignature;
        private string? _operationSummary;
        private Func<Task>? _prepareDocumentAsync;
        private Func<Task>? _refreshVisibleChunkAsync;
        private Action<bool>? _setMainDocumentBusy;

        public searchResults()
        {
            InitializeComponent();
            InitializeSearchLogic();
            ApplyLocalization();
        }

        public IReadOnlyList<SearchResult> SearchResults => _searchResults;

        public event EventHandler<SearchResultEventArgs>? NavigateToResultRequested;

        public void ConfigureDocumentCallbacks(Func<Task>? prepareDocumentAsync, Func<Task>? refreshVisibleChunkAsync, Action<bool>? setMainDocumentBusy)
        {
            _prepareDocumentAsync = prepareDocumentAsync;
            _refreshVisibleChunkAsync = refreshVisibleChunkAsync;
            _setMainDocumentBusy = setMainDocumentBusy;
        }

        public void SetSearchTerm(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTextBox.Text = searchTerm;
                searchTextBox.SelectAll();
            }
        }

        public void FocusSearchBox()
        {
            searchTextBox.Focus();
        }

        public void ResetResults()
        {
            _searchResults = [];
            _lastSearchSignature = null;
            _operationSummary = null;
            resultsListView.Items.Clear();
            UpdateSummary();
            UpdateResultButtonsState();
        }

        public void ApplyLocalization()
        {
            Text = LocalizationManager.Get("search.form.title", "Suchen und Ersetzen");
            replaceLabel.Text = LocalizationManager.Get("search.replace_label", "Ersetzen durch");
            matchCaseCheckBox.Text = LocalizationManager.Get("search.match_case", "Gross/Klein beachten");
            wholeWordCheckBox.Text = LocalizationManager.Get("search.whole_word", "Whole Word");
            interpretEscapesCheckBox.Text = LocalizationManager.Get("search.interpret_escapes", "Escapes deuten");
            searchButton.Text = LocalizationManager.Get("search.search_button", "Suchen");
            replaceButton.Text = LocalizationManager.Get("search.replace_button", "Ersetzen");
            replaceAllButton.Text = LocalizationManager.Get("search.replace_all_button", "Alle ersetzen");
            chunkColumn.Text = LocalizationManager.Get("search.column.chunk", "Chunk");
            positionColumn.Text = LocalizationManager.Get("search.column.position", "Position");
            previewColumn.Text = LocalizationManager.Get("search.column.preview", "Vorschau");
            UpdateSummary();
        }

        public void SetDocumentBusy(bool isBusy)
        {
            searchTextBox.Enabled = !isBusy;
            replaceTextBox.Enabled = !isBusy;
            matchCaseCheckBox.Enabled = !isBusy;
            wholeWordCheckBox.Enabled = !isBusy;
            interpretEscapesCheckBox.Enabled = !isBusy;
            searchButton.Enabled = !isBusy;
            replaceButton.Enabled = !isBusy;
            replaceAllButton.Enabled = !isBusy;
            resultsListView.Enabled = !isBusy;
        }

        private void InitializeSearchLogic()
        {
            interpretEscapesCheckBox.Checked = true;
            searchButton.Click += searchButton_Click;
            replaceButton.Click += replaceButton_Click;
            replaceAllButton.Click += replaceAllButton_Click;
            resultsListView.ItemActivate += resultsListView_ItemActivate;
            resultsListView.SelectedIndexChanged += resultsListView_SelectedIndexChanged;
            searchTextBox.TextChanged += searchCriteriaControl_Changed;
            matchCaseCheckBox.CheckedChanged += searchCriteriaControl_Changed;
            wholeWordCheckBox.CheckedChanged += searchCriteriaControl_Changed;
            interpretEscapesCheckBox.CheckedChanged += searchCriteriaControl_Changed;
            AcceptButton = searchButton;
            UpdateSummary();
            UpdateResultButtonsState();
        }

        private async void searchButton_Click(object? sender, EventArgs e)
        {
            await ExecuteSearchAsync();
        }

        private async void replaceButton_Click(object? sender, EventArgs e)
        {
            if (!EnsureDocumentIsOpen())
            {
                return;
            }

            try
            {
                SearchQueryOptions query = BuildQuery();
                string replacementText = BuildReplacementText();

                await PrepareDocumentIfNeededAsync();
                await EnsureCurrentResultsAsync(query);

                if (_searchResults.Count == 0)
                {
                    ShowInfo(LocalizationManager.Get("search.info.no_matches_to_replace", "Es wurden keine Treffer zum Ersetzen gefunden."));
                    return;
                }

                SearchResult target = GetSelectedResultOrFirst();
                ReplaceResult replaceResult = await RunDocumentOperationAsync(
                    () => ProgressDialogRunner.RunAsync(
                        this,
                        LocalizationManager.Get("search.progress.replace_one", "Treffer wird ersetzt"),
                        (progress, cancellationToken) => Textfile.ReplaceAsync(query, target, replacementText, progress, cancellationToken)));

                await RefreshResultsAsync(
                    query,
                    replaceResult.Changed
                        ? LocalizationManager.Get("search.operation.replaced_one", "1 Treffer ersetzt.")
                        : LocalizationManager.Get("search.operation.replacement_unchanged", "Der Treffer entsprach bereits dem Ersatztext."),
                    replaceResult.ReplacementLocation);

                SearchResult navigationTarget = GetSelectedResultOrDefault() ?? replaceResult.ReplacementLocation;
                NavigateToResultRequested?.Invoke(this, new SearchResultEventArgs(navigationTarget));
            }
            catch (OperationCanceledException)
            {
                summaryLabel.Text = LocalizationManager.Get("search.summary.replace_cancelled", "Ersetzen wurde abgebrochen.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, LocalizationManager.Get("search.replace_button", "Ersetzen"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void replaceAllButton_Click(object? sender, EventArgs e)
        {
            if (!EnsureDocumentIsOpen())
            {
                return;
            }

            try
            {
                SearchQueryOptions query = BuildQuery();
                string replacementText = BuildReplacementText();

                await PrepareDocumentIfNeededAsync();
                ReplaceAllResult replaceAllResult = await RunDocumentOperationAsync(
                    () => ProgressDialogRunner.RunAsync(
                        this,
                        LocalizationManager.Get("search.progress.replace_all", "Alle Treffer werden ersetzt"),
                        (progress, cancellationToken) => Textfile.ReplaceAllAsync(query, replacementText, progress, cancellationToken)));

                if (_refreshVisibleChunkAsync is not null)
                {
                    await _refreshVisibleChunkAsync();
                }

                string operationSummary = replaceAllResult.MatchCount == 0
                    ? LocalizationManager.Get("search.operation.replace_all_none", "Keine Treffer zum Ersetzen gefunden.")
                    : LocalizationManager.Format("search.operation.replace_all_summary", "{0:N0} von {1:N0} Treffer(n) ersetzt.", replaceAllResult.ChangedCount, replaceAllResult.MatchCount);

                await RefreshResultsAsync(query, operationSummary);
            }
            catch (OperationCanceledException)
            {
                summaryLabel.Text = LocalizationManager.Get("search.summary.replace_all_cancelled", "Alle ersetzen wurde abgebrochen.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, LocalizationManager.Get("search.replace_button", "Ersetzen"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void resultsListView_ItemActivate(object? sender, EventArgs e)
        {
            SearchResult? selectedResult = GetSelectedResultOrDefault();
            if (selectedResult is not null)
            {
                NavigateToResultRequested?.Invoke(this, new SearchResultEventArgs(selectedResult));
            }
        }

        private void resultsListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateResultButtonsState();
        }

        private void searchCriteriaControl_Changed(object? sender, EventArgs e)
        {
            _lastSearchSignature = null;
            _operationSummary = null;
            UpdateSummary();
            UpdateResultButtonsState();
        }

        private async Task ExecuteSearchAsync()
        {
            if (!EnsureDocumentIsOpen())
            {
                return;
            }

            try
            {
                SearchQueryOptions query = BuildQuery();
                await PrepareDocumentIfNeededAsync();
                await RefreshResultsAsync(query);
            }
            catch (OperationCanceledException)
            {
                summaryLabel.Text = LocalizationManager.Get("search.summary.search_cancelled", "Suche wurde abgebrochen.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, LocalizationManager.Get("search.search_button", "Suche"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PrepareDocumentIfNeededAsync()
        {
            if (_prepareDocumentAsync is not null)
            {
                await _prepareDocumentAsync();
            }
        }

        private async Task EnsureCurrentResultsAsync(SearchQueryOptions query)
        {
            if (_lastSearchSignature == BuildSearchSignature())
            {
                return;
            }

            await RefreshResultsAsync(query);
        }

        private async Task RefreshResultsAsync(SearchQueryOptions query, string? operationSummary = null, SearchResult? anchorResult = null)
        {
            IReadOnlyList<SearchResult> results = await RunDocumentOperationAsync(
                () => ProgressDialogRunner.RunAsync(
                    this,
                    LocalizationManager.Get("search.progress.search", "Suche laeuft"),
                    (progress, cancellationToken) => Textfile.SearchAsync(query, progress, cancellationToken)));

            _searchResults = results.ToList();
            _lastSearchSignature = BuildSearchSignature();
            _operationSummary = operationSummary;
            PopulateResults(anchorResult);
        }

        private async Task<T> RunDocumentOperationAsync<T>(Func<Task<T>> operation)
        {
            SetDocumentBusy(true);
            _setMainDocumentBusy?.Invoke(true);

            try
            {
                return await operation();
            }
            finally
            {
                _setMainDocumentBusy?.Invoke(false);
                SetDocumentBusy(false);
                UpdateResultButtonsState();
            }
        }

        private SearchQueryOptions BuildQuery()
        {
            if (searchTextBox.Text.Length == 0)
            {
                throw new InvalidOperationException(LocalizationManager.Get("search.info.enter_search_term", "Bitte zuerst einen Suchbegriff eingeben."));
            }

            return new SearchQueryOptions(
                searchTextBox.Text,
                matchCaseCheckBox.Checked,
                wholeWordCheckBox.Checked,
                interpretEscapesCheckBox.Checked);
        }

        private string BuildReplacementText()
        {
            string rawReplacementText = replaceTextBox.Text;
            return interpretEscapesCheckBox.Checked ? SearchTextParser.Unescape(rawReplacementText) : rawReplacementText;
        }

        private bool EnsureDocumentIsOpen()
        {
            if (Textfile.HasOpenFile)
            {
                return true;
            }

            ShowInfo(LocalizationManager.Get("search.info.no_file", "Es ist aktuell keine Datei geoeffnet."));
            return false;
        }

        private string BuildSearchSignature()
        {
            return string.Join(
                "\u001F",
                searchTextBox.Text,
                matchCaseCheckBox.Checked,
                wholeWordCheckBox.Checked,
                interpretEscapesCheckBox.Checked);
        }

        private SearchResult GetSelectedResultOrFirst()
        {
            return GetSelectedResultOrDefault()
                ?? _searchResults.FirstOrDefault()
                ?? throw new InvalidOperationException(LocalizationManager.Get("search.error.no_selected_result", "Es ist aktuell kein Suchtreffer ausgewaehlt."));
        }

        private SearchResult? GetSelectedResultOrDefault()
        {
            return resultsListView.SelectedItems.Count == 0
                ? null
                : resultsListView.SelectedItems[0].Tag as SearchResult;
        }

        private void PopulateResults(SearchResult? anchorResult = null)
        {
            resultsListView.BeginUpdate();

            try
            {
                resultsListView.Items.Clear();

                foreach (SearchResult result in _searchResults)
                {
                    var item = new ListViewItem(LocalizationManager.Format("search.result.chunk_item", "Chunk {0}", result.ChunkIndex + 1));
                    item.SubItems.Add(result.PositionInChunk.ToString("N0"));
                    item.SubItems.Add(result.Preview);
                    item.Tag = result;
                    resultsListView.Items.Add(item);
                }

                SelectResultItem(anchorResult);
            }
            finally
            {
                resultsListView.EndUpdate();
            }

            UpdateSummary();
            UpdateResultButtonsState();
        }

        private void SelectResultItem(SearchResult? anchorResult)
        {
            if (resultsListView.Items.Count == 0)
            {
                return;
            }

            int selectedIndex = anchorResult is null ? 0 : FindNextResultIndex(anchorResult);
            if (selectedIndex < 0 || selectedIndex >= resultsListView.Items.Count)
            {
                selectedIndex = 0;
            }

            resultsListView.Items[selectedIndex].Selected = true;
            resultsListView.Items[selectedIndex].Focused = true;
            resultsListView.EnsureVisible(selectedIndex);
        }

        private int FindNextResultIndex(SearchResult anchorResult)
        {
            int anchorOffset = anchorResult.PositionInChunk + Math.Max(anchorResult.MatchLength, 0);

            for (int index = 0; index < _searchResults.Count; index++)
            {
                SearchResult result = _searchResults[index];
                if (result.ChunkIndex > anchorResult.ChunkIndex)
                {
                    return index;
                }

                if (result.ChunkIndex == anchorResult.ChunkIndex && result.PositionInChunk >= anchorOffset)
                {
                    return index;
                }
            }

            return _searchResults.Count > 0 ? 0 : -1;
        }

        private void UpdateSummary()
        {
            string baseSummary = _searchResults.Count == 0
                ? LocalizationManager.Get("search.summary.none", "Keine Treffer geladen. Escapes wie \\n, \\t, \\u0041 sind optional verfuegbar.")
                : LocalizationManager.Format("search.summary.results", "{0:N0} Treffer gefunden. Doppelklick springt zur Stelle.", _searchResults.Count);

            summaryLabel.Text = string.IsNullOrWhiteSpace(_operationSummary)
                ? baseSummary
                : $"{_operationSummary} {baseSummary}";
        }

        private void UpdateResultButtonsState()
        {
            bool hasSearchTerm = searchTextBox.TextLength > 0;
            bool hasOpenDocument = Textfile.HasOpenFile;
            bool hasSelection = resultsListView.SelectedItems.Count > 0 || _searchResults.Count > 0;

            searchButton.Enabled = hasOpenDocument && hasSearchTerm;
            replaceButton.Enabled = hasOpenDocument && hasSearchTerm && hasSelection;
            replaceAllButton.Enabled = hasOpenDocument && hasSearchTerm;
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, LocalizationManager.Get("search.form.title", "Suche"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
