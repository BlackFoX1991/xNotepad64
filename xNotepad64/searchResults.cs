namespace xNotepad64
{
    public partial class searchResults : Form
    {
        private List<SearchResult> _searchResults = [];

        public searchResults()
        {
            InitializeComponent();
            InitializeSearchLogic();
        }

        public IReadOnlyList<SearchResult> SearchResults => _searchResults;

        public event EventHandler<SearchResultEventArgs>? NavigateToResultRequested;

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
            resultsListView.Items.Clear();
            UpdateSummary();
        }

        public void SetDocumentBusy(bool isBusy)
        {
            searchTextBox.Enabled = !isBusy;
            matchCaseCheckBox.Enabled = !isBusy;
            wholeWordCheckBox.Enabled = !isBusy;
            interpretEscapesCheckBox.Enabled = !isBusy;
            searchButton.Enabled = !isBusy;
            resultsListView.Enabled = !isBusy;
        }

        private void InitializeSearchLogic()
        {
            interpretEscapesCheckBox.Checked = true;
            searchButton.Click += searchButton_Click;
            resultsListView.ItemActivate += resultsListView_ItemActivate;
            AcceptButton = searchButton;
            UpdateSummary();
        }

        private async void searchButton_Click(object? sender, EventArgs e)
        {
            string rawSearchTerm = searchTextBox.Text;
            if (rawSearchTerm.Length == 0)
            {
                MessageBox.Show("Bitte zuerst einen Suchbegriff eingeben.", "Suche", MessageBoxButtons.OK, MessageBoxIcon.Information);
                searchTextBox.Focus();
                return;
            }

            if (!Textfile.HasOpenFile)
            {
                MessageBox.Show("Es ist aktuell keine Datei geoeffnet.", "Suche", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var query = new SearchQueryOptions(
                    rawSearchTerm,
                    matchCaseCheckBox.Checked,
                    wholeWordCheckBox.Checked,
                    interpretEscapesCheckBox.Checked);

                SetDocumentBusy(true);
                IReadOnlyList<SearchResult> results = await ProgressDialogRunner.RunAsync(
                    this,
                    "Suche laeuft",
                    (progress, cancellationToken) => Textfile.SearchAsync(query, progress, cancellationToken));

                _searchResults = results.ToList();
                PopulateResults();
            }
            catch (OperationCanceledException)
            {
                summaryLabel.Text = "Suche wurde abgebrochen.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Suche", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetDocumentBusy(false);
            }
        }

        private void resultsListView_ItemActivate(object? sender, EventArgs e)
        {
            if (resultsListView.SelectedItems.Count == 0)
            {
                return;
            }

            if (resultsListView.SelectedItems[0].Tag is SearchResult result)
            {
                NavigateToResultRequested?.Invoke(this, new SearchResultEventArgs(result));
            }
        }

        private void PopulateResults()
        {
            resultsListView.BeginUpdate();

            try
            {
                resultsListView.Items.Clear();

                foreach (SearchResult result in _searchResults)
                {
                    var item = new ListViewItem($"Chunk {result.ChunkIndex + 1}");
                    item.SubItems.Add(result.PositionInChunk.ToString("N0"));
                    item.SubItems.Add(result.Preview);
                    item.Tag = result;
                    resultsListView.Items.Add(item);
                }
            }
            finally
            {
                resultsListView.EndUpdate();
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            summaryLabel.Text = _searchResults.Count == 0
                ? "Keine Treffer geladen. Escapes wie \\n, \\t, \\u0041 sind optional verfuegbar."
                : $"{_searchResults.Count:N0} Treffer gefunden. Doppelklick springt zur Stelle.";
        }
    }
}
