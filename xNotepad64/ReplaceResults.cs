namespace xNotepad64
{
    public sealed class ReplaceResult
    {
        public required SearchResult ReplacementLocation { get; init; }

        public bool Changed { get; init; }
    }

    public sealed class ReplaceAllResult
    {
        public int MatchCount { get; init; }

        public int ChangedCount { get; init; }
    }
}
