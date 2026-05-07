namespace xNotepad64
{
    public sealed class SearchResultEventArgs : EventArgs
    {
        public SearchResultEventArgs(SearchResult result)
        {
            Result = result;
        }

        public SearchResult Result { get; }
    }
}
