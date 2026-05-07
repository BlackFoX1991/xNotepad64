namespace xNotepad64
{
    internal sealed class ChunkEditHistory
    {
        private readonly List<ChunkTextState> _states = [];
        private readonly int _maxSnapshots;
        private int _position;

        public ChunkEditHistory(string initialText, int selectionStart, int selectionLength, int maxSnapshots = 20)
        {
            _maxSnapshots = Math.Max(2, maxSnapshots);
            _states.Add(new ChunkTextState(initialText, selectionStart, selectionLength));
        }

        public ChunkTextState CurrentState => _states[_position];

        public bool CanUndo => _position > 0;

        public bool CanRedo => _position < _states.Count - 1;

        public void ReplaceCurrentSelection(int selectionStart, int selectionLength)
        {
            _states[_position] = CurrentState with
            {
                SelectionStart = selectionStart,
                SelectionLength = selectionLength
            };
        }

        public void RecordState(string text, int selectionStart, int selectionLength)
        {
            ChunkTextState current = CurrentState;
            if (string.Equals(current.Text, text, StringComparison.Ordinal))
            {
                ReplaceCurrentSelection(selectionStart, selectionLength);
                return;
            }

            if (CanRedo)
            {
                _states.RemoveRange(_position + 1, _states.Count - (_position + 1));
            }

            _states.Add(new ChunkTextState(text, selectionStart, selectionLength));

            if (_states.Count > _maxSnapshots)
            {
                _states.RemoveAt(0);
            }
            else
            {
                _position++;
                return;
            }

            _position = _states.Count - 1;
        }

        public ChunkTextState Undo()
        {
            if (!CanUndo)
            {
                return CurrentState;
            }

            _position--;
            return CurrentState;
        }

        public ChunkTextState Redo()
        {
            if (!CanRedo)
            {
                return CurrentState;
            }

            _position++;
            return CurrentState;
        }
    }

    internal readonly record struct ChunkTextState(string Text, int SelectionStart, int SelectionLength);
}
