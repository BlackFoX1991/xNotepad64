namespace xNotepad64
{
    public sealed class OperationProgress
    {
        public OperationProgress(string stage, string detail, long completed = 0, long total = 0)
        {
            Stage = stage;
            Detail = detail;
            Completed = completed;
            Total = total;
        }

        public string Stage { get; }

        public string Detail { get; }

        public long Completed { get; }

        public long Total { get; }

        public bool IsIndeterminate => Total <= 0;

        public int Percent => IsIndeterminate
            ? 0
            : (int)Math.Clamp(Math.Round((double)Completed / Total * 100d), 0d, 100d);
    }
}
