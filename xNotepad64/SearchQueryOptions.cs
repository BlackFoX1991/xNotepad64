namespace xNotepad64
{
    public sealed class SearchQueryOptions
    {
        public SearchQueryOptions(string rawSearchTerm, bool matchCase, bool wholeWord, bool interpretEscapes)
        {
            if (rawSearchTerm is null)
            {
                throw new ArgumentNullException(nameof(rawSearchTerm));
            }

            RawSearchTerm = rawSearchTerm;
            MatchCase = matchCase;
            WholeWord = wholeWord;
            InterpretEscapes = interpretEscapes;
            SearchTerm = interpretEscapes ? SearchTextParser.Unescape(rawSearchTerm) : rawSearchTerm;

            if (SearchTerm.Length == 0)
            {
                throw new ArgumentException("A non-empty search term is required.", nameof(rawSearchTerm));
            }
        }

        public string RawSearchTerm { get; }

        public string SearchTerm { get; }

        public bool MatchCase { get; }

        public bool WholeWord { get; }

        public bool InterpretEscapes { get; }
    }
}
