namespace Game2048.Core
{
    /// <summary>
    /// Đại diện cho một ô trong lưới game 2048.
    /// Pure C# class — không phụ thuộc Unity runtime.
    /// </summary>
    public class Tile
    {
        public int Value { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        /// <summary>Đánh dấu ô đã gộp trong nước đi hiện tại để tránh gộp 2 lần.</summary>
        public bool IsMerged { get; set; }

        public Tile(int row, int col, int value)
        {
            Row = row;
            Col = col;
            Value = value;
            IsMerged = false;
        }

        public void ResetMergeFlag() => IsMerged = false;

        public override string ToString() => $"Tile({Row},{Col})={Value}";
    }
}