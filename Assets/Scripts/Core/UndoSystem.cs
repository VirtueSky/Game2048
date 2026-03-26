namespace Game2048.Core
{
    /// <summary>
    /// Hỗ trợ undo 1 nước đi bằng cách lưu snapshot BoardState trước khi move.
    /// </summary>
    public class UndoSystem
    {
        private BoardState _snapshot;

        public bool HasSnapshot => _snapshot != null;

        /// <summary>Lưu snapshot trước khi thực thi nước đi.</summary>
        public void SaveSnapshot(BoardState state)
        {
            _snapshot = state.Clone();
        }

        /// <summary>
        /// Khôi phục snapshot. Trả về snapshot nếu có, null nếu không.
        /// Snapshot bị xóa sau khi undo để tránh undo liên tiếp.
        /// </summary>
        public BoardState Undo()
        {
            if (_snapshot == null) return null;
            var result = _snapshot;
            _snapshot = null;
            return result;
        }

        public void Clear() => _snapshot = null;
    }
}